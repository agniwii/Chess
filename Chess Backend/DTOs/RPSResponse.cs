namespace Chess_Backend.DTOs
{
    public class RPSResponse
    {
        public string? Winner { get; set; }
        public string? Loser { get; set; }
        public bool IsDraw { get; set; }
        public required string Message { get; set; }
    }
}