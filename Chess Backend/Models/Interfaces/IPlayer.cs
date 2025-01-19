namespace Chess_Backend.Models.Interfaces
{
    public interface IPlayer
    {
        string Name { get; set; }
        string ConnectionId { get; set; }
        Color Color { get; set; }
        List<Piece> CapturedPieces { get; set; }

        public void AddCapturedPiece(Piece piece);
    }
}