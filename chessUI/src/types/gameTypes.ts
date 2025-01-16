export interface Cell
{
    player: string;
}

export type Board = Array<Array<Cell>>;

export interface GameState
{
    board: Board;
    player: string[];
    moves: Array<{player: string, move: string}>;
}

export interface signalRState {
    gameId: string|null;
    player: string|null;
    connected: boolean;
}

export interface RootState{
    game: GameState;
    signalR: signalRState;
}

export const gameId = "caturTutorial";