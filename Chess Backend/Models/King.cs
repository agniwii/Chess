namespace Chess_Backend.Models
{
    class King(Color color, Position position) : Piece(color, position)
    {
        public override bool IsValidMove(Position newPosition, Piece?[,] board)
        {
            int dx = Math.Abs(newPosition.X - Position.X);
            int dy = Math.Abs(newPosition.Y - Position.Y);

            if (dx > 1 || dy > 1)
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
