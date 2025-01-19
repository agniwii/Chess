using Microsoft.AspNetCore.SignalR;
using Chess_Backend.Services;
using Chess_Backend.Models;
using System.Collections.Immutable;
using Chess_Backend.DTOs;
using Chess_Backend.Services.Interfaces;

namespace Chess_Backend.Hubs
{
    public class ChessHub : Hub
    {
        private readonly IChessService _chessService;
        private readonly IRPSService _rpsService;

        public ChessHub(IChessService chessService, IRPSService rpsService)
        {
            _chessService = chessService;
            _rpsService = rpsService;
        }

        public async Task JoinGame(string gameId, string playerName)
        {
            if (_chessService.GetTotalPlayers(gameId) >= 2)
            {
                await Clients.Caller.SendAsync("GameFull", "Game is full, please try again later.");
                return;
            }

            if (_chessService.GetGame(gameId) == null)
            {
                var game = new ChessGame(gameId);
                _chessService.AddGame(gameId, game);
            }

            var player = new Player(playerName, Color.None, Context.ConnectionId);
            _chessService.AddPlayer(gameId, player);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Client(Context.ConnectionId).SendAsync("PlayerJoined", gameId);
            await Clients.Clients(Context.ConnectionId).SendAsync("PlayerId", Context.ConnectionId);

            if (_chessService.GetTotalPlayers(gameId) == 1)
            {
                // Send loading state to the first player
                await Clients.Caller.SendAsync("Loading", "Waiting for another player to join...");
            }
            else if (_chessService.GetTotalPlayers(gameId) == 2)
            {
                // Start the game when two players have joined
                await StartGame(gameId);
            }
        }

        public async Task StartGame(string gameId)
        {
            var players = _chessService.GetPlayers(gameId).ToImmutableList();
            var player1 = players[0];
            var player2 = players[1];

            var response = new
            {
                GameId = gameId,
                CurrentPlayer = "White",
                Players = new[]
                {
                    new { Name = player1.Name, Color = player1.Color.ToString() },
                    new { Name = player2.Name, Color = player2.Color.ToString() }
                }
            };

            // Notify both players that the game is ready
            await Clients.Group(gameId).SendAsync("GameReady", response);

            // Start RPS after GameReady
            await StartRPS(gameId);
        }

        private async Task StartRPS(string gameId)
        {
            await Clients.Group(gameId).SendAsync("StartRPS", "Choose Rock, Paper, or Scissors to determine who plays as White.");
        }

        public async Task SubmitChoice(RPSRequest request)
        {
            var response = await _rpsService.SubmitChoice(request);

            await Clients.Group(request.GameId).SendAsync("RPSResult", response);
            if (response.Winner != null)
            {
                var players = _chessService.GetPlayers(request.GameId).ToImmutableList();
                var player1 = players[0];
                var player2 = players[1];

                player1.Color = response.Winner == player1.Name ? Color.White : Color.Black;
                player2.Color = response.Winner == player2.Name ? Color.White : Color.Black;

                await Clients.Client(player1.ConnectionId).SendAsync("RPSResult", $"{player1.Name} gets {(response.Winner == player1.Name ? "White" : "Black")} piece");
                await Clients.Client(player2.ConnectionId).SendAsync("RPSResult", $"{player2.Name} gets {(response.Winner == player2.Name ? "White" : "Black")} piece");

                // Start the chess game after RPS is resolved
                await Clients.Group(request.GameId).SendAsync("StartChess", "The chess game is starting!");
            }
            else
            {
                await Clients.Group(request.GameId).SendAsync("RPSResult", "Draw");
                // Restart RPS if it's a draw
                await StartRPS(request.GameId);
            }
        }

        public async Task MakeChoice(string gameId, string choice)
        {
            _chessService.AddPlayerChoice(gameId, Context.ConnectionId, choice);

            var players = _chessService.GetPlayers(gameId).ToImmutableList();
            var choices = _chessService.GetPlayerChoices(gameId);

            if (choices.Count == 2)
            {
                var player1 = players[0];
                var player2 = players[1];

                var winner = _chessService.DetermineRPSWinner(choices[player1.ConnectionId ?? ""], choices[player2.ConnectionId ?? ""]);

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

                await StartGame(gameId);
            }
        }

        public async Task MovePiece(MoveRequest request)
        {
            var from = new Position(request.From.X, request.From.Y);
            var to = new Position(request.To.X, request.To.Y);
            string gameId = request.GameId;
            string playerId = request.PlayerId;

            var result = _chessService.MakeMove(gameId, playerId, from, to);

            if (result.IsValidMove)
            {
                await Clients.Group(gameId).SendAsync("ReceiveMove", result);
                await UpdateCurrentPlayer(gameId);
            }
            else
            {
                await Clients.Caller.SendAsync("InvalidMove", result.Message);
            }
        }

        public async Task GetPossibleMoves(string gameId, int x, int y)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
            {
                await Clients.Caller.SendAsync("ReceivePossibleMoves", "Invalid position: x and y must be between 0 and 7.");
                return;
            }

            var position = new Position(x, y);
            var possibleMoves = _chessService.GetPossibleMoves(gameId, position);

            if (possibleMoves == null || possibleMoves.PossibleMoves.Count == 0)
            {
                if (possibleMoves?.PieceType == "None")
                {
                    await Clients.Caller.SendAsync("ReceivePossibleMoves", "No piece found at the given position.");
                }
                else
                {
                    await Clients.Caller.SendAsync("ReceivePossibleMoves", $"No possible moves found for the {possibleMoves?.PieceType}.");
                }
                return;
            }

            await Clients.Caller.SendAsync("ReceivePossibleMoves", possibleMoves);
        }

        public async Task UpdateCurrentPlayer(string gameId)
        {
            var currentPlayer = _chessService.GetCurrentPlayer(gameId);
            await Clients.Group(gameId).SendAsync("UpdateCurrentPlayer", currentPlayer);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}