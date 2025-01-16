import { Piece } from "../types";
import { Position } from "./Position";

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
    from: Position;
    to: Position;
}