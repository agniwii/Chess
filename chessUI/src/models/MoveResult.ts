import { Piece } from "../types";

export interface MoveResult {
    isValidMove: boolean;
    message: string;
    isCheck: boolean;
    isCheckmate: boolean;
    isStalemate: boolean;
    capturedPiece: Piece[] | null;
    isDraw: boolean;
    isPromotion: boolean;
    isEnPassant: boolean;
    isCastling: boolean;
}