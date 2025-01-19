using System.Collections.Generic;
using Chess_Backend.Models;
using Chess_Backend.DTOs;
using Chess_Backend.Models.Interfaces;

namespace Chess_Backend.Services
{
    public interface IChessService
    {
        void AddPlayer(string gameId, IPlayer player);
        int GetTotalPlayers(string gameId);
        IEnumerable<Player> GetPlayers(string gameId);
        ChessGame? GetGame(string gameId);
        void AddGame(string gameId, ChessGame game);
        MoveResult MakeMove(string gameId, string playerId, Position from, Position to);
        PossibleMovesResponse GetPossibleMoves(string gameId, Position position);
        void AddPlayerChoice(string gameId, string connectionId, string choice);
        Dictionary<string, string> GetPlayerChoices(string gameId);
        void ResetPlayerChoices(string gameId);
        string DetermineRPSWinner(string choice1, string choice2);
        string GetCurrentPlayer(string gameId);
    }
}