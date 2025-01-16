namespace Chess_Backend.Models
{
    public class Pawn : Piece
    {
        public Pawn(Color color, Position position) : base(color, position) { }
        public override bool IsValidMove(Position newPosition, Piece?[,] board)
        {
            int dx = newPosition.X - Position.X;
            int dy = newPosition.Y - Position.Y;
            var targetPiece = board[newPosition.X, newPosition.Y];

            int forward = Color == Color.White ? 1 : -1;
            int startingRow = Color == Color.White ? 1 : 6;

            if (dx == 0 && dy == forward && targetPiece == null)
                return true;

            if (Position.Y == startingRow && dx == 0 && dy == 2 * forward && 
                targetPiece == null && board[newPosition.X, newPosition.Y - forward] == null)
                return true;

            if (dy == forward && Math.Abs(dx) == 1 && targetPiece != null && 
                targetPiece.Color != Color)
                return true;
            return false;
        }
    }
}
