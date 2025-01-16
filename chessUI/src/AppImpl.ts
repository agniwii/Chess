import { Piece, PieceType, Color } from './types';

export class AppImpl {
    private board: (Piece | null)[][];

    constructor() {
        this.board = this.initializeBoard();
    }
    private initializeBoard(): (Piece | null)[][] {
        const board: (Piece | null)[][] = Array(8).fill(null).map(() => Array(8).fill(null));

        // Initialize pawns
        for (let i = 0; i < 8; i++) {
            board[i][1] = { type: PieceType.Pawn, color: Color.White, position: { x: i, y: 1 } };
            board[i][6] = { type: PieceType.Pawn, color: Color.Black, position: { x: i, y: 6 } };
        }

        const backRankPieces = [
            PieceType.Rook,
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.Queen,
            PieceType.King,
            PieceType.Bishop,
            PieceType.Knight,
            PieceType.Rook
        ];

        for (let i = 0; i < 8; i++) {
            board[i][0] = { type: backRankPieces[i], color: Color.White, position: { x: i, y: 0 } };
            board[i][7] = { type: backRankPieces[i], color: Color.Black, position: { x: i, y: 7 } };
        }

        return board;
    }

    getBoard(): (Piece | null)[][] {
        return this.board;
    }
}