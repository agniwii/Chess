import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Position } from "../../models";

interface ChessState {
    gameId: string | null;
    board: (string | null)[][];
    selectedPosition: Position | null;
    possibleMoves: Position[];
    currentPlayerId: string | null;
}

const initialState: ChessState = {
    gameId: null,
    board: Array(8).fill(Array(8).fill(null)),
    selectedPosition: null,
    possibleMoves: [],
    currentPlayerId: null,
};

const chessSlice = createSlice({
    name: "chess",
    initialState,
    reducers: {
        setGameId(state, action: PayloadAction<string | null>) {
            state.gameId = action.payload;
        },
        setBoard(state, action: PayloadAction<(string | null)[][]>) {
            state.board = action.payload;
        },
        setSelectedPosition(state, action: PayloadAction<Position | null>) {
            state.selectedPosition = action.payload;
        },
        setPossibleMoves(state, action: PayloadAction<Position[]>) {
            state.possibleMoves = action.payload;
        },
        movePiece(state, action: PayloadAction<{ from: Position; to: Position }>) {
            const { from, to } = action.payload;
            const piece = state.board[from.x][from.y];
            state.board[from.x][from.y] = null;
            state.board[to.x][to.y] = piece;
        },
        setCurrentPlayerId(state, action: PayloadAction<string | null>) {
            state.currentPlayerId = action.payload;
        },
        resetGame(state) {
            state.board = Array(8).fill(Array(8).fill(null));
            state.selectedPosition = null;
            state.possibleMoves = [];
            state.currentPlayerId = null;
        },
    },
});

export const {
    setGameId,
    setBoard,
    setSelectedPosition,
    setPossibleMoves,
    movePiece,
    setCurrentPlayerId,
    resetGame,
} = chessSlice.actions;

export default chessSlice.reducer;