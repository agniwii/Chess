using System.Collections;
using Chess_Backend.Models;
using Chess_Backend.DTOs;
namespace Chess_Backend.Services
{
    public class ChessService
    {

        public Dictionary<string, ChessGame> _game = new();
        // buat class khusus untuk menyimpan pilihan pemain
        private Dictionary<string, Dictionary<string, string>> _playerChoices = new();
        // private List<KeyValuePair<string,List<KeyValuePair<string, string>>>> _playerChoices = new List<KeyValuePair<string,List<KeyValuePair<string, string>>>>();
        private IPlayer? player;

        // menerima instant player dari luar dari hub dan buat interface untuk aturannya agar menjadi 
        // lebih fleksibel public void AddPlayer(IPlayer player)
        public void AddPlayer(string gameId, IPlayer players)
        {
            if (!_game.ContainsKey(gameId))
            {
                _game.Add(gameId, new ChessGame(gameId));
            }
            _game[gameId].Players.Add(players);
        }

        public int GetTotalPlayers(string gameId)
        {
            return _game.ContainsKey(gameId) ? _game[gameId].Players.Count : 0;
        }

        // ganti menjadi IEnumerable<Player> agar lebih fleksibel
        public IEnumerable<Player> GetPlayers(string gameId)
        {
            return _game.ContainsKey(gameId) ? _game[gameId].Players.Select(p => (Player)p) : Enumerable.Empty<Player>();
        }

        public ChessGame? GetGame(string gameId)
        {
            return _game.ContainsKey(gameId) ? _game[gameId] : null;
        }

        public MoveResult MakeMove(string gameId,string playerId, Position from, Position to)
        {
            if (!_game.ContainsKey(gameId))
            {
                // throw new ArgumentException("Game not found", nameof(gameId));
                return new MoveResult
                {
                    IsValidMove = false,
                    Message = "Game not found"
                };
            }
            if(_game[gameId].CurrentPlayer.ToString() != playerId)
            {
                return new MoveResult
                {
                    IsValidMove = false,
                    Message = "It's not your turn"
                };
            }

            Console.WriteLine("Before move:");
            _game[gameId].PrintBoard();
            var result = _game[gameId].MakeMove(from, to);
            Console.WriteLine("After move:");
            _game[gameId].PrintBoard();
            return result;
        }

        public PossibleMovesResponse GetPossibleMoves(string gameId, Position position)
        {
            var piece = _game[gameId].Board[position.X, position.Y];
            if (piece == null || piece.Color != _game[gameId].CurrentPlayer)
            {
                return new PossibleMovesResponse
                {
                    PossibleMoves = [],
                    PieceType = "None",
                    PieceColor = "None",
                };
            }
            Console.WriteLine($"Getting possible moves for {piece.GetType().Name} at {position.X}, {position.Y}");

            var possibleMoves = new List<Position>();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Position newPosition = new(x, y);
                    if (piece.IsValidMove(newPosition, _game[gameId].Board))
                    {
                        var tempBoard = (Piece?[,])_game[gameId].Board.Clone();
                        tempBoard[newPosition.X, newPosition.Y] = piece;
                        tempBoard[position.X, position.Y] = null;
                        if (!_game[gameId].IsKingInCheck(tempBoard, _game[gameId].CurrentPlayer))
                        {
                            possibleMoves.Add(newPosition);
                        }
                    }
                }
            }


            return new PossibleMovesResponse
            {
                PossibleMoves = possibleMoves,
                PieceType = piece
                    .GetType()
                    .Name,
                PieceColor = piece.Color.ToString(),
            };
        }
        public void AddPlayerChoice(string gameId, string connectionId, string choice)
        {
            if (!_playerChoices.ContainsKey(gameId)) _playerChoices.Add(gameId, new Dictionary<string, string>());
            if (!_playerChoices[gameId].ContainsKey(connectionId)) _playerChoices[gameId].Add(connectionId, choice);
            _playerChoices[gameId][connectionId] = choice;
        }

        // ubah jadi key value pair agar lebih fleksibel
        // public List<KeyValuePair<string, string>> GetPlayerChoice(string gameId, string connectionId)
        // {
        //     // Kembalikan pilihan pemain untuk gameId tertentu
        //     return _playerChoices.ContainsKey(gameId) ? _playerChoices[gameId] : new List<KeyValuePair<string, string>>();
        // }s
        public Dictionary<string, string> GetPlayerChoices(string gameId)
        {
            // Kembalikan pilihan pemain untuk gameId tertentu
            return _playerChoices.ContainsKey(gameId) ? _playerChoices[gameId] : new Dictionary<string, string>();
        }

        public void ResetPlayerChoices(string gameId)
        {
            // Reset pilihan pemain untuk gameId tertentu
            if (_playerChoices.ContainsKey(gameId))
            {
                _playerChoices[gameId].Clear();
            }
        }
        public string DetermineRPSWinner(string choice1, string choice2)
        {
            if (choice1 == choice2)
            {
                return "Draw";
            }

            if ((choice1 == "Rock" && choice2 == "Scissors") ||
                (choice1 == "Paper" && choice2 == "Rock") ||
                (choice1 == "Scissors" && choice2 == "Paper"))
            {
                return "Player1";
            }

            return "Player2";
        }

        public string GetCurrentPlayer(string gameId)
        {
            string playerColor = _game[gameId].CurrentPlayer.ToString();
            if (player != null && playerColor == player.Color.ToString())
            {
                var currentPlayer = _game[gameId].Players.Find(p => p.Color.ToString() == playerColor.ToString());
                return currentPlayer != null ? currentPlayer.Name : "Unknown";
            }
            return string.Empty;
        }
    }
}
