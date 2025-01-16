namespace Chess_Backend.Models
{
    public class MoveResult
    {
        public bool IsValidMove { get; set; }
        public string? Message { get; set; }
        public bool IsCheck { get; set; }
        public bool IsCheckmate { get; set; }
        public bool IsStalemate { get; set; }
        public Piece? CapturedPiece { get; set; }
        public bool IsDraw { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsEnPassant { get; set; }
        public bool IsCastling { get; set; }
    }
}
