import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { createConnection, startConnection, stopConnection } from './utilities/network';
import { RootState, AppDispatch } from './store/store';
import { Modals } from './components/Modals';
import { JoinGame } from './pages/joinGame';
import { Board } from './components';
import './App.css';
import { getPossibleMoves, makeMove, joinGame } from './features';

function App() {
    const dispatch = useDispatch<AppDispatch>();
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const { gameId, currentPlayerId, selectedPosition } = useSelector((state: RootState) => state.chess);

    // Mulai koneksi SignalR saat komponen dimount
    useEffect(() => {
        const url = "http://localhost:5042/chesshub";
        const connection = createConnection(url);
        startConnection(connection).then(() => setConnection(connection));

        // Handle cleanup saat komponen unmount
        return () => {
            if (connection) {
                stopConnection(connection);
            }
        };
    }, [dispatch]);

    // Handle join game
    const handleJoinGame = async (playerName: string) => {
        if (connection) {
            const gameId = "game1"; // Ganti dengan game ID yang sesuai
            await dispatch(joinGame({ connection, gameId, playerName }));
            
        } else {
            console.error("Connection is not established.");
        }
    };

    // Handle square click di papan
    const handleSquareClick = async (x: number, y: number) => {
        if (!currentPlayerId || !gameId) {
            console.log("No player or game ID is currently active.");
            return;
        }

        if (selectedPosition) {
            // Lakukan pergerakan bidak
            const moveRequest = {
                gameId,
                playerId: currentPlayerId,
                from: selectedPosition,
                to: { x, y },
            };
            if (connection) {
                await dispatch(makeMove({ connection, moveRequest }));
            }
        } else {
            // Dapatkan possible moves untuk bidak yang dipilih
            if (connection) {
                await dispatch(getPossibleMoves({ connection, gameId, x, y, playerId: currentPlayerId }));
            }
        }
    };

    return (
        <>
            <div className='Container'>
                <h1>Chess</h1>
                <JoinGame onJoin={handleJoinGame} />
            </div>

            {connection && (
                <Modals isOpen={!!gameId} connectionId={connection} gameId={gameId || ''}>
                    {gameId ? (
                        <>
                            <h2>Game ID: {gameId}</h2>
                            <h3>Current Player: {currentPlayerId}</h3>
                            <Board onSquareClick={handleSquareClick} />
                        </>
                    ) : (
                        <div className='Loading-Container'>
                            <h2>Waiting for player 2</h2>
                            <div className="loader"></div>
                        </div>
                    )}
                </Modals>
            )}
        </>
    );
}

export default App;