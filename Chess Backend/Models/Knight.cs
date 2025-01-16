namespace Chess_Backend.Models
{
    public class Knight(Color color, Position position) : Piece(color, position)
    {
        public override bool IsValidMove(Position newPosition, Piece?[,] board)
        {
            int dx = Math.Abs(newPosition.X - Position.X);
            int dy = Math.Abs(newPosition.Y - Position.Y);

            if (!((dx == 2 && dy == 1) || (dx == 1 && dy == 2)))
            {
                return false;
            }

            if (!newPosition.IsValid())
            {
                return false;
            }

            var targetPiece = board[newPosition.X, newPosition.Y];
            if (targetPiece != null && targetPiece.Color == Color)
            {
                return false;
            }

            return true;
        }

    }
}
