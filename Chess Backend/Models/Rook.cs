namespace Chess_Backend.Models
{
    class Rook(Color color, Position position) : Piece(color, position)
    {
        public override bool IsValidMove(Position newPosition, Piece?[,] board)
        {
            int dx = Math.Abs(newPosition.X - Position.X);
            int dy = Math.Abs(newPosition.Y - Position.Y);

            if (dx != 0 && dy != 0)
            {
                return false;
            }

            if (!newPosition.IsValid())
            {
                return false;
            }

            if (IsPathBlocked(newPosition, board))
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

        private bool IsPathBlocked(Position newPosition, Piece?[,] board)
        {
            int x = Position.X;
            int y = Position.Y;
            
            int xDirection = newPosition.X > x ? 1 : (newPosition.X < y ? -1 : 0);
            int yDirection = newPosition.Y > y ? 1 : (newPosition.Y < y ? -1 : 0);
            
            x +=+ xDirection;
            y +=+ yDirection;

            while (x != newPosition.X || y != newPosition.Y)
            {
                if (board[x, y] != null)
                {
                    return true;
                }
                x += xDirection;
                y += yDirection;
            }

            return false;
        }
    }
}
