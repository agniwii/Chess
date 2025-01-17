import { Position } from "../models";

export interface Cell
{
    player: string;
}

export type Board = Array<Array<Cell>>;



interface ChessState {

    gameId: string | null;

    currentPlayerId: string | null;

    selectedPosition: Position | null;

    possibleMoves: Position[];

    board: Board;
}

export interface signalRState {
    gameId: string|null;
    player: string|null;
    connected: boolean;
}

export interface RootState{
    game: ChessState;
    signalR: signalRState;
}

export const gameId = "caturTutorial";