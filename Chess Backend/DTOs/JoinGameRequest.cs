namespace Chess_Backend.DTOs
{
    public class JoinGameRequest
    {
        public required string GameId { get; set; }
        public required string PlayerName { get; set; }
    }
}