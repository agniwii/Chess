import { createAsyncThunk } from "@reduxjs/toolkit";
import { createConnection,startConenection,onServerMethod, invokeServerMethod, stopConnection } from "../../utilities";
import { setPossibleMoves, movePiece } from "./chessSlice";
import { PossibleMovesResponse,MoveResult, Position,MoveRequest } from "../../models";
import { RootState } from "../../store/store";

export const startSignalRConnection = createAsyncThunk(
    "chess/startSignalRConnection",
    async(url:string, {dispatch}) =>{
        const connection = createConnection(url);
        await startConenection(connection);

        // listen for possible moves
        onServerMethod(connection, "ReceivePossibleMoves", (...args: unknown[]) => {
            const response = args[0] as PossibleMovesResponse;
            dispatch(setPossibleMoves(response.possibleMoves));
        }
        );

        onServerMethod(connection, "ReceiveMoveResult", (...args: unknown[]) => {
            const result = args[0] as MoveResult;
            if(result.isValidMove){
                const fromPosition: Position = JSON.parse(result.message);
                const toPosition: Position = JSON.parse(result.to as unknown as string);
                dispatch(movePiece({from: fromPosition, to: toPosition}));
            }
        }
        );
        return connection;
    }
);

export const getPossibleMoves = createAsyncThunk(
    'chess/getPossibleMoves',
    async (
        { connection, gameId, x, y, playerId }: 
        { connection: signalR.HubConnection; gameId: string; x: number; y: number; playerId: string },
        { rejectWithValue }
    ) => {
        try {
            await invokeServerMethod(connection, 'GetPossibleMoves', gameId, x, y, playerId);
        } catch (err) {
            return rejectWithValue(err);
        }
    }
);

export const makeMove = createAsyncThunk(
    'chess/makeMove',
    async (
        { connection, moveRequest }: 
        { connection: signalR.HubConnection; moveRequest: MoveRequest },
        {  getState,rejectWithValue }
    ) => {
        const state = getState() as RootState;
        const currentPlayerId = state.chess.currentPlayerId;
        if(moveRequest.playerId !== currentPlayerId ){
            return rejectWithValue("It is not your turn");
        }
        try {
            await invokeServerMethod(connection, 'MovePiece', moveRequest);
        } catch (err) {
            return rejectWithValue(err);
        }
    }
);

export const stopSignalRConnection = createAsyncThunk(
    'chess/stopSignalRConnection',
    async (connection: signalR.HubConnection, { rejectWithValue }) => {
        try {
            await stopConnection(connection);
        } catch (err) {
            return rejectWithValue(err);
        }
    }
);