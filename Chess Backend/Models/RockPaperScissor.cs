namespace Chess_Backend.Models
{
    public enum RockPaperScissorChoice
    {
        Rock,
        Paper,
        Scissor
    }

    public class RockPaperScissor
    {
        public RockPaperScissor(string winner, string loser, bool isDraw)
        {
            WinnerConnectionId = winner;
            LoserConnectionId = loser;
            IsDraw = isDraw;
        }
        public string? WinnerConnectionId { get; set; }
        public string? LoserConnectionId { get; set; }
        public bool IsDraw { get; set; }
    }
}