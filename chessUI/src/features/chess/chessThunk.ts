import { createAsyncThunk } from "@reduxjs/toolkit";
import { createConnection,startConenection,onServerMethod,offServerMethod } from "../../utilities";
import { setPossibleMoves, movePiece } from "./chessSlice";
import { PossibleMovesResponse,MoveResult } from "../../models";

export const startSignalRConnection = createAsyncThunk(
    "chess/startSignalRConnection",
    async(url:string, {dispatch}) =>{
        const connection = createConnection(url);
        await startConenection(connection);

        // listen for possible moves
        onServerMethod(connection, "ReceivePossibleMoves", (response: PossibleMovesResponse) => {
            dispatch(setPossibleMoves(response.possibleMoves));
        }
        );

        onServerMethod(connection, "ReceiveMoveResult", (result: MoveResult) => {
            if(result.isValidMove){
                dispatch(movePiece({from: result.message., to: result.to}));
            }
        }
        );