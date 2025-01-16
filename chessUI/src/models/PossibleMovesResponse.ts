import { Position } from "./Position";

export interface PossibleMovesResponse {
    possibleMoves: Position[];
    gameId: string;
    pieceColor: string;
}