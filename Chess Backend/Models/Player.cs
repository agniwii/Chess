namespace Chess_Backend.Models
{
    public class Player : IPlayer
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<Piece> CapturedPieces { get; set; }


        public Player(string name, Color color, string connectionId)
        {
            Name = name;
            Color = color;
            ConnectionId = connectionId;
            CapturedPieces = new List<Piece>();
        }

        public void AddCapturedPiece(Piece piece)
        {
            CapturedPieces.Add(piece);
        }
    }
}