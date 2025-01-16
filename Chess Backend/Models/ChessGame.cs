using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chess_Backend.Models
{
    public class ChessGame
    {
        public Piece?[,] Board { get; set; } = new Piece?[8, 8];
        public Color CurrentPlayer { get; private set; }
        public bool IsCheck { get; private set; }
        public bool IsCheckmate { get; private set; }
        public bool IsStalemate { get; private set; }
        public bool IsDraw { get; private set; }
        public bool IsPromotion { get; private set; }
        public bool IsEnPassant { get; private set; }
        public bool IsCastling { get; private set; }

        public string GameId { get; set; }

        public List<IPlayer> Players { get; set; } = [];
        // ubah jadi array karena hanya 2 pemain
        // public Array<Player> Players { get; set; } = new Player[2];

        public ChessGame(string gameId)
        {
            GameId = gameId;
            Players = [];
            InitializeBoard();
            PrintBoard();
            CurrentPlayer = Color.White;
        }

        // public void AddPlayer(Player player, Color color)
        // {
        //     Players.Add(player,color);
        // }

        private void InitializeBoard()
        {
            Board[0, 0] = new Rook(Color.White, new Position(0, 0));
            Board[1, 0] = new Knight(Color.White, new Position(1, 0));
            Board[2, 0] = new Bishop(Color.White, new Position(2, 0));
            Board[3, 0] = new Queen(Color.White, new Position(3, 0));
            Board[4, 0] = new King(Color.White, new Position(4, 0));
            Board[5, 0] = new Bishop(Color.White, new Position(5, 0));
            Board[6, 0] = new Knight(Color.White, new Position(6, 0));
            Board[7, 0] = new Rook(Color.White, new Position(7, 0));

            for (int i = 0; i < 8; i++)
            {
                Board[i, 1] = new Pawn(Color.White, new Position(i, 1));
                Board[i, 6] = new Pawn(Color.Black, new Position(i, 6));
            }

            Board[0, 7] = new Rook(Color.Black, new Position(0, 7));
            Board[1, 7] = new Knight(Color.Black, new Position(1, 7));
            Board[2, 7] = new Bishop(Color.Black, new Position(2, 7));
            Board[3, 7] = new Queen(Color.Black, new Position(3, 7));
            Board[4, 7] = new King(Color.Black, new Position(4, 7));
            Board[5, 7] = new Bishop(Color.Black, new Position(5, 7));
            Board[6, 7] = new Knight(Color.Black, new Position(6, 7));
            Board[7, 7] = new Rook(Color.Black, new Position(7, 7));

            for (int i = 0; i < 8; i++)
            {
                for (int j = 2; j < 6; j++)
                {
                    Board[i, j] = null;
                }
            }
        }

        public MoveResult MakeMove(Position from, Position to)
        {
            var pieces = Board[from.X, from.Y];
            if (pieces == null || pieces.Color != CurrentPlayer)
            {
                return new MoveResult { IsValidMove = false, Message = "Invalid move: Not your piece." };
            }

            if (!pieces.IsValidMove(to, Board))
            {
                return new MoveResult { IsValidMove = false, Message = "Invalid move: Illegal move" };
            }

            var targetPiece = Board[to.X, to.Y];
            bool isCapture = targetPiece != null && targetPiece.Color != CurrentPlayer;

            var tempBoard = (Piece?[,])Board.Clone();
            tempBoard[to.X, to.Y] = pieces;
            tempBoard[from.X, from.Y] = null;

            if (IsKingInCheck(tempBoard, CurrentPlayer))
            {
                return new MoveResult { IsValidMove = false, Message = "Invalid move: King is in check" };
            }

            Board = tempBoard;
            pieces.Position = to;

            IsCheck = IsKingInCheck(
                Board,
                CurrentPlayer == Color.White ? Color.Black : Color.White
            );
            IsCheckmate =
                IsCheck
                && !HasAnyValidMove(CurrentPlayer == Color.White ? Color.Black : Color.White);
            IsStalemate =
                !IsCheck
                && !HasAnyValidMove(CurrentPlayer == Color.White ? Color.Black : Color.White);
            IsDraw = IsStalemate || IsCheckmate;

            CurrentPlayer = CurrentPlayer == Color.White ? Color.Black : Color.White;

            return new MoveResult
            {
                IsValidMove = true,
                Message = isCapture ? "Move Successful. Enemy piece captured." : "Move Successful.",
                IsCheck = IsCheck,
                IsCheckmate = IsCheckmate,
                IsStalemate = IsStalemate,
                CapturedPiece = isCapture ? targetPiece : null,
            };
        }

        public bool IsKingInCheck(Piece?[,] board, Color color)
        {
            var kingPosition = GetKingPosition(board, color);
            if (kingPosition == null) return false;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    if (
                        piece != null
                        && piece.Color != color
                        && piece.IsValidMove(kingPosition, board)
                    )
                    {
                        Console.WriteLine($"King is in check by {piece.GetType().Name} at {i}, {j}");
                        return true;
                    }
                }
            }
            Console.WriteLine("King is not in check");
            return false;
        }

        private Position? GetKingPosition(Piece?[,] board, Color color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = board[i, j];
                    if (piece != null && piece.Color == color && piece is King)
                    {
                        Console.WriteLine($"King position: {i}, {j}");
                        return new Position(i, j);
                    }
                }
            }
            return null;
        }

        private bool HasAnyValidMove(Color color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = Board[i, j];
                    if (piece != null && piece.Color == color)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                if (piece.IsValidMove(new Position(x, y), Board))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void PrintBoard()
        {
            Console.WriteLine("  a b c d e f g h");
            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1} ");
                for (int x = 0; x < 8; x++)
                {
                    var piece = Board[x, y];
                    if (piece == null)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write($"{GetPieceSymbol(piece)} ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
        }

        private string GetPieceSymbol(Piece piece)
        {
            switch (piece)
            {
                case King k:
                    return k.Color == Color.White ? "K" : "k";
                case Queen q:
                    return q.Color == Color.White ? "Q" : "q";
                case Rook r:
                    return r.Color == Color.White ? "R" : "r";
                case Bishop b:
                    return b.Color == Color.White ? "B" : "b";
                case Knight n:
                    return n.Color == Color.White ? "N" : "n";
                case Pawn p:
                    return p.Color == Color.White ? "P" : "p";
                default:
                    return ".";
            }
        }
    }
}
