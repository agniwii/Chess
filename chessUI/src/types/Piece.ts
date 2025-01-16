export type PieceType = "King" | "Queen" | "Rook" | "Bishop" | "Knight" | "Pawn";


export type PieceColor = "White" | "Black";


export interface Piece {
    type: PieceType;
    color: PieceColor;
    position: { x: number, y: number };
}