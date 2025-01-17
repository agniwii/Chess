
namespace Chess_Backend.DTOs
{
    public enum RockPaperScissorChoice
    {
        Rock,
        Paper,
        Scissor
    }
    public class RPSRequest
    {
        public required string GameId { get; set; }
        public required string PlayerId { get; set; }
        public RockPaperScissorChoice Choice { get; set; }
    }    
}