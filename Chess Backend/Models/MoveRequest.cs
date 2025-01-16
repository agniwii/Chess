namespace Chess_Backend.Models
{
    public class MoveRequest
    {
        public string? GameId { get; set; }
        public string? PlayerId { get; set; }
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
    }
}
