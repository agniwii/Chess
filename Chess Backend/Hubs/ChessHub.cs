// tambahin signalR di sini
using Microsoft.AspNetCore.SignalR;
using Chess_Backend.Services;
using Chess_Backend.Models;
using System.Collections.Immutable;

namespace Chess_Backend.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ChessService _chessService;


        public ChessHub(ChessService chessService)
        {
            _chessService = chessService;
        }


        public async Task JoinGame(string gameId, string playerName)
        {
            if (_chessService.GetTotalPlayers(gameId) >= 2)
            {
                await Clients.Caller.SendAsync("GameFull", "Game is full, please try again later.");
                return;
            }
            if(_chessService.GetGame(gameId) == null)
            {
                var game = new ChessGame(gameId);
                _chessService._game.Add(gameId, game);
            }
            _chessService.AddPlayer(gameId, new Player(playerName, Color.None, Context.ConnectionId));    
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Client(Context.ConnectionId).SendAsync("PlayerJoined", gameId);
            if (_chessService.GetTotalPlayers(gameId) == 2)
            {
                await StartGame(gameId);
            }
        }

        public async Task StartGame(string gameId)
        {
            var response = new
            {
                GameId = gameId,
                CurrentPlayer = "White",
                Players = _chessService?.GetPlayers(gameId)
            };
            await Clients.Group(gameId).SendAsync("GameReady", response);

        }

        public async Task MakeChoice(string gameId, string choice)
        {
            _chessService.AddPlayerChoice(gameId, Context.ConnectionId, choice);

            var players = _chessService.GetPlayers(gameId).ToImmutableList();
            var choices = _chessService.GetPlayerChoices(gameId);
            // ubah method untuk mengambil player 1 dan player 2 
            if (choices.Count() == 2)
            {
                var player1 = players[0];
                var player2 = players[1];

                var winner = DetermineRPSWinner(choices[player1.ConnectionId ?? ""], choices[player2.ConnectionId ?? ""]);
                if (winner == "Draw")
                {
                    await Clients.Client(player1.ConnectionId ?? "").SendAsync("RPSResult", "Draw");
                    await Clients.Client(player2.ConnectionId ?? "").SendAsync("RPSResult", "Draw");
                    _chessService.ResetPlayerChoices(gameId);
                    return;
                }

                player1.Color = winner == "Player1" ? Color.White : Color.Black;
                player2.Color = winner == "Player2" ? Color.White : Color.Black;

                await Clients.Client(player1.ConnectionId ?? "").SendAsync("RPSResult", $"{player1.Name} gets {(winner == "Player1" ? "White" : "Black")} piece");
                await Clients.Client(player2.ConnectionId ?? "").SendAsync("RPSResult", $"{player2.Name} gets {(winner == "Player2" ? "White" : "Black")} piece");

                _chessService.ResetPlayerChoices(gameId);
            }
        }

        private string DetermineRPSWinner(string choice1, string choice2)
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


        public async Task MovePiece(MoveRequest request)
        {
            var from = new Position(request.FromX, request.FromY);
            var to = new Position(request.ToX, request.ToY);
            string? gameId = request.GameId ?? string.Empty;
            var result = _chessService.MakeMove(gameId,from, to);
            await Clients.All.SendAsync("ReceiveMove", result);
        }

        public async Task GetPossibleMoves(string gameId, int x, int y)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
            {
                await Clients.Caller.SendAsync("ReceivePossibleMoves", "Invalid position: x and y must be between 0 and 7.");
                return;
            }

            var position = new Position(x, y);
            if (_chessService == null)
            {
                await Clients.Caller.SendAsync("ReceivePossibleMoves", "Chess service is not available.");
                return;
            }
            var possibleMoves = _chessService.GetPossibleMoves(gameId, position);

            if (possibleMoves == null || possibleMoves.PossibleMoves.Count == 0)
            {
                if (possibleMoves?.PieceType == "None") await Clients.Caller.SendAsync("ReceivePossibleMoves", "No piece found at the given position.");
                else await Clients.Caller.SendAsync("ReceivePossibleMoves", $"No possible moves found for the {possibleMoves?.PieceType}.");
                return;
            }
            await Clients.Caller.SendAsync("ReceivePossibleMoves", position);
        }
    }
}