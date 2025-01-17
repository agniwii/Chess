using Chess_Backend.Models;

namespace Chess_Backend.DTOs
{
    public class MoveResponse
    {
        public required Position From { get; set; }
        public required Position To { get; set; }
    }
}