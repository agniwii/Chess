import { createAsyncThunk } from "@reduxjs/toolkit";
import { createConnection, startConnection, stopConnection, invokeServerMethod, onServerMethod } from "../../utilities/network";
import { setPossibleMoves, movePiece, setCurrentPlayerId, resetGame } from "./chessSlice";
import { PossibleMovesResponse, MoveResult, Position, MoveRequest } from "../../models";
import { RootState } from "../../store/store";

// Thunk untuk memulai koneksi SignalR
export const startSignalRConnection = createAsyncThunk(
    "chess/startSignalRConnection",
    async (url: string, { dispatch }) => {
        const connection = createConnection(url);
        await startConnection(connection);

        // Dengarkan event dari backend
        onServerMethod(connection, "ReceivePossibleMoves", (...args: unknown[]) => {
            const response = args[0] as PossibleMovesResponse;
            dispatch(setPossibleMoves(response.possibleMoves));
        });

        onServerMethod(connection, "ReceiveMoveResult", (...args: unknown[]) => {
            const result = args[0] as MoveResult;
            if (result.isValidMove) {
                const fromPosition: Position = JSON.parse(result.message);
                const toPosition: Position = JSON.parse(result.to as unknown as string);
                dispatch(movePiece({ from: fromPosition, to: toPosition }));
            }
        });

        onServerMethod(connection, "PlayerJoined", (...args: unknown[]) => {
            const playerId = args[0] as string;
            dispatch(setCurrentPlayerId(playerId));
        });

        onServerMethod(connection, "GameReady", (...args: unknown[]) => {
            const gameData = args[0] as {
                gameId: string;
                currentPlayer: string;
                players: {
                    connectionId: string;
                    name: string;
                    color: number;
                    capturedPieces: string[] | null;
                }[];
            };
            dispatch(setCurrentPlayerId(gameData.currentPlayer));
        });

        onServerMethod(connection, "GameOver", (...args: unknown[]) => {
            const winner = args[0] as string;
            dispatch(resetGame());
            alert(`Game over! Winner: ${winner}`);
        });

        return connection;
    }
);

// Thunk untuk bergabung ke permainan
export const joinGame = createAsyncThunk(
    "chess/joinGame",
    async (
        { connection, gameId, playerName }: 
        { connection: signalR.HubConnection; gameId: string; playerName: string },
        { rejectWithValue }
    ) => {
        try {
            await invokeServerMethod(connection, "JoinGame", gameId, playerName);
        } catch (err) {
            console.error("Error joining game:", err);
            return rejectWithValue(err);
        }
    }
);

// Thunk untuk mendapatkan possible moves
export const getPossibleMoves = createAsyncThunk(
    "chess/getPossibleMoves",
    async (
        { connection, gameId, x, y, playerId }: 
        { connection: signalR.HubConnection; gameId: string; x: number; y: number; playerId: string },
        { getState, rejectWithValue }
    ) => {
        const state = getState() as RootState;
        const currentPlayerId = state.chess.currentPlayerId;

        if (playerId !== currentPlayerId) {
            return rejectWithValue("It is not your turn");
        }

        try {
            await invokeServerMethod(connection, "GetPossibleMoves", gameId, x, y, playerId);
        } catch (err) {
            return rejectWithValue(err);
        }
    }
);

// Thunk untuk melakukan pergerakan bidak
export const makeMove = createAsyncThunk(
    "chess/makeMove",
    async (
        { connection, moveRequest }: 
        { connection: signalR.HubConnection; moveRequest: MoveRequest },
        { dispatch, getState, rejectWithValue }
    ) => {
        const state = getState() as RootState;
        const currentPlayerId = state.chess.currentPlayerId;

        if (moveRequest.playerId !== currentPlayerId) {
            return rejectWithValue("It is not your turn");
        }

        try {
            await invokeServerMethod(connection, "MovePiece", moveRequest);

            // Update giliran pemain
            const nextPlayerId = currentPlayerId === "player1" ? "player2" : "player1";
            dispatch(setCurrentPlayerId(nextPlayerId));
        } catch (err) {
            return rejectWithValue(err);
        }
    }
);

// Thunk untuk memutuskan koneksi SignalR
export const stopSignalRConnection = createAsyncThunk(
    "chess/stopSignalRConnection",
    async (connection: signalR.HubConnection, { dispatch, rejectWithValue }) => {
        try {
            await stopConnection(connection);

            // Reset state saat disconnect
            dispatch(resetGame());
        } catch (err) {
            return rejectWithValue(err);
        }
    }
);