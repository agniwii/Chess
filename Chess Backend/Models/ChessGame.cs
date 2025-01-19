using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Chess_Backend.DTOs;
using Chess_Backend.Models.Interfaces;

namespace Chess_Backend.Models
{
    public class ChessGame
    {
        public Piece?[,] Board { get; set; }
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
            Players = new List<IPlayer>();
            Board = new Piece?[8, 8];
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
                return new MoveResult { IsValidMove = false, Message = "Invalid move: Illegal move"};
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
            pieces.MoveCount++;

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
                From = from,
                To = to,
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

        private static Position? GetKingPosition(Piece?[,] board, Color color)
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
            // Get all pieces of the given color
            var pieces = Board.Cast<Piece?>()
                    .Where(p => p?.Color == color);

            // For each piece, check if it has any valid moves
            foreach (var piece in pieces)
            {
            // Get potential moves within board bounds
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                if (piece?.IsValidMove(new Position(x, y), Board) == true)
                {
                    return true;
                }
                }
            }
            }
            return false;
        }

        public void MakeCastling(Position king, Position Rook)
        {
            // Check if the king and rook are in their initial positions
            if (king.Y != Rook.Y)
            {
                return;
            }

            // Check if the squares between the king and rook are empty
            int row = king.Y;
            int rookColumn = Math.Min(king.X, Rook.X);
            int kingColumn = Math.Max(king.X, Rook.X);

            // Check if squares between rook and king are empty for queenside castling
            for (int col = rookColumn + 1; col < kingColumn; col++)
            {
                if (Board[col, row] != null)
                {
                    return;
                }
            }

            // Move the king and rook
            Board[kingColumn, row] = Board[king.X, row];
            Board[king.X, row] = null;
            Board[rookColumn, row] = Board[Rook.X, row];
            Board[Rook.X, row] = null;
        }

        public bool IsCastlingMove()
        {
            if(IsCheck) 
            {
                IsCastling = false;
                return false;
            }
            if (Board.Cast<Piece?>().FirstOrDefault(p => p is King && p.Color == CurrentPlayer) is King king && king.MoveCount > 0) 
            {
                IsCastling = false;
                return false;
            }
            if(Board.Cast<Piece?>().FirstOrDefault(p => p is Rook && p.Color == CurrentPlayer) is Rook rook && rook.MoveCount > 0) 
            {
                IsCastling = false;
                return false;
            }
            if (IsKingInCheck(Board, CurrentPlayer)) 
            {
                IsCastling = false;
                return false;
            }
            // check my rook to king is empty
            int row = CurrentPlayer == Color.White ? 0 : 7;
            int rookColumn = 0;
            int kingColumn = 4;

            // Check if squares between rook and king are empty for queenside castling
            for (int col = rookColumn + 1; col < kingColumn; col++)
            {
                if (Board[col, row] != null)
                {
                    IsCastling = false;
                    return false;
                }
            }

            rookColumn = 7;

            // Check if squares between king and rook are empty for kingside castling
            for (int col = kingColumn + 1; col < rookColumn; col++)
            {
                if (Board[col, row] != null)
                {
                    IsCastling = false;
                    return false;
                }
            }
            IsCastling = true;
            return true;
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

        private static string GetPieceSymbol(Piece piece)
        {
            return piece switch
            {
                King k => k.Color == Color.White ? "K" : "k",
                Queen q => q.Color == Color.White ? "Q" : "q",
                Rook r => r.Color == Color.White ? "R" : "r",
                Bishop b => b.Color == Color.White ? "B" : "b",
                Knight n => n.Color == Color.White ? "N" : "n",
                Pawn p => p.Color == Color.White ? "P" : "p",
                _ => ".",
            };
        }
    }
}
