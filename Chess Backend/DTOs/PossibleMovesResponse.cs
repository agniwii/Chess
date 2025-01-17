using Chess_Backend.Models;
namespace Chess_Backend.DTOs
{
    public class PossibleMovesResponse
    {
        public required List<Position> PossibleMoves { get; set; }

        public required string PieceType { get; set; }

        public required string PieceColor { get; set; }
    }
}
