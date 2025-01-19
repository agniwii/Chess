import React, { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';
import { createConnection, startConnection, stopConnection, onServerMethod, invokeServerMethod } from '../utilities/network.ts';

interface SignalRContextType {
    connection: signalR.HubConnection | null;
    joinRoom: (gameId: string, playerName: string) => Promise<void>;
    playerName: string;
    setPlayerName: (name: string) => void;
    playerId: string;
    setPlayerId: (id: string) => void;
    gameStatus: string;
    setGameStatus: (status: string) => void;
    submitRPSChoice: (gameId: string, playerId: string, choice: string) => Promise<void>;
    movePiece: (gameId: string, playerId: string, from: string, to: string) => Promise<void>;
}

interface SignalRProviderProps {
    children: ReactNode;
}

const SignalRContext = createContext<SignalRContextType>({
    connection: null,
    joinRoom: async () => {},
    playerId: '',
    setPlayerId: () => {},
    playerName: '',
    gameStatus: '',
    setGameStatus: () => {},
    setPlayerName: () => {},
    submitRPSChoice: async () => {},
    movePiece: async () => {},
});

export const SignalRProvider: React.FC<SignalRProviderProps> = ({ children }) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [playerId, setPlayerId] = useState('');
    const [gameStatus, setGameStatus] = useState<string>('');
    const [playerName, setPlayerName] = useState<string>('');

    useEffect(() => {
        const newConnection = createConnection('http://localhost:5042/chesshub');

        startConnection(newConnection).then(() => {
            setConnection(newConnection);

            // Handle PlayerId
            onServerMethod(newConnection, 'PlayerId', (...args: unknown[]) => {
                const id = args[0] as string;
                setPlayerId(id);
            });

            // Handle GameStatus
            onServerMethod(newConnection, 'GameStatus', (...args: unknown[]) => {
                const status = args[0] as string;
                setGameStatus(status);
                console.log('Game status:', status);
            });

            // Handle Loading
            onServerMethod(newConnection, 'Loading', (...args: unknown[]) => {
                const message = args[0] as string;
                setGameStatus('Loading');
                console.log(message);
            });

            // Handle GameReady
            onServerMethod(newConnection, 'GameReady', (response: any) => {
                setGameStatus('GameReady');
                console.log('Game is ready:', response);
            });

            // Handle StartRPS
            onServerMethod(newConnection, 'StartRPS', (...args: unknown[]) => {
                const message = args[0] as string;
                setGameStatus('RPSStarted');
                console.log(message);
            });

            onServerMethod(newConnection, 'GameFull', (...args: unknown[]) => {
                const message = args[0] as string;
                setGameStatus('GameFull');
                console.log(message);
            });

            // Handle RPSResult
            onServerMethod(newConnection, 'RPSResult', (...args: unknown[]) => {
                const result = args[0] as string;
                setGameStatus('RPSResult');
                console.log('RPS Result:', result);
            });

            // Handle StartChess
            onServerMethod(newConnection, 'StartChess', (...args: unknown[]) => {
                const message = args[0] as string;
                setGameStatus('ChessStarted');
                console.log(message);
            });
        });

        return () => {
            if (newConnection) {
                stopConnection(newConnection);
            }
        };
    }, []);

    const joinRoom = async (gameId: string, playerName: string) => {
        if (connection) {
            try {
                await invokeServerMethod(connection, 'JoinGame', gameId, playerName);
                console.log(`Player ${playerName} joined room ${gameId}`);
            } catch (err) {
                console.error('Error joining room:', err);
            }
        }
    };

    const submitRPSChoice = async (gameId: string, playerId: string, choice: string) => {
        if (connection) {
            try {
                await invokeServerMethod(connection, 'SubmitChoice', gameId, playerId, choice);
            } catch (err) {
                console.error('Error submitting RPS choice:', err);
            }
        }
    };


    const movePiece = async (gameId: string, playerId: string, from: string, to: string) => {
        if (connection) {
            try {
                await invokeServerMethod(connection, 'MovePiece', gameId, playerId, from, to);
            } catch (err) {
                console.error('Error moving piece:', err);
            }
        }
    };

    return (
        <SignalRContext.Provider
            value={{
                connection,
                joinRoom,
                submitRPSChoice,
                gameStatus,
                setGameStatus,
                playerName,
                setPlayerName,
                movePiece,
                playerId,
                setPlayerId,
            }}
        >
            {children}
        </SignalRContext.Provider>
    );
};

export const useSignalR = () => useContext(SignalRContext);