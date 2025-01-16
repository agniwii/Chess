export enum PieceType {
        King = "King",
    Queen = "Queen",
    Rook = "Rook",
    Bishop = "Bishop",
    Knight = "Knight",
    Pawn = "Pawn"
}

export enum Color {
    White = "White",
    Black = "Black"
}


export interface Piece {
    type: PieceType;
    color: Color;
    position: { x: number, y: number };
}