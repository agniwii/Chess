// import { useEffect, useState } from 'react'
import { useEffect, useState } from 'react';
import './App.css';
import { Modals } from './components';
import { JoinGame } from './pages/joinGame';
import { createConnection, invokeServerMethod, startConenection, onServerMethod } from './utilities/network';
import signalR from '@microsoft/signalr';
import { gameId as ID } from './types/gameTypes';


function App() {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [gameData, setGameData] = useState<{
        gameId: string | null,
        currentPlayer: string | null,
        players: {
            connectionId: string,
            name: string,
            color: number,
            capturedPieces: string[] | null;
        }[];
    } | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    useEffect(() => {
        const newConnection = createConnection("http://localhost:5042/chesshub");
        setConnection(newConnection);

        startConenection(newConnection).then(() => {
            console.log('connected');
        })
        onServerMethod(newConnection, "PlayerJoined", (...args: unknown[]) => {
            const message = args[0] as string;
            setIsModalOpen(true);
            console.log("ini dari backend", message);
        });
        onServerMethod(newConnection, "GameReady", (...args: unknown[]) => {
            const gameData = args[0] as {
                gameId: string | null,
                currentPlayer: string | null,
                players: {
                    connectionId: string,
                    name: string,
                    color: number,
                    capturedPieces: string[] | null;
                }[];
            };
            setGameData(gameData);
            console.log("ini dari backend", gameData);
        });
        onServerMethod(newConnection, "GameFull", (...args: unknown[]) => {
            const message = args[0] as string;
            alert(message);
        });
    }, [])
    const handleJoinGame = async (playerName: string) => {
        if (connection) {
            await invokeServerMethod(connection, "JoinGame", ID, playerName);
        } else {
            console.error("Connection is not established.");
        }
    };
    return (
        <>
            <div className='Container'>
                <h1>Chess</h1>
                <JoinGame onJoin={handleJoinGame} />
            </div>

            {connection && <Modals isOpen={isModalOpen} connectionId={connection} gameId={gameData?.gameId || ''}>
                {isModalOpen && gameData != null? null
                
                    : isModalOpen && gameData == null ?
                    <>
                    <div className='Loading-Container'>
                        <h2>waiting for player 2</h2>
                        <div className="loader"></div>
                    </div>
                </>
                : null
                }
            </Modals>}
        </>
    );
}

export default App
