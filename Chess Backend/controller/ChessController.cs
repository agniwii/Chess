using Chess_Backend.Models;
using Chess_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chess_Backend.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChessController : ControllerBase
    {
        private readonly ChessService _chessService;

        public ChessController(ChessService chessService)
        {
            _chessService = chessService;
        }

        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] MoveRequest request)
        {
            var from = new Position(request.FromX, request.FromY);
            var to = new Position(request.ToX, request.ToY);
            string? gameId = request.GameId ?? string.Empty;
            var result = _chessService.MakeMove(gameId,from, to);
            return Ok(result);
        }


        [HttpGet("possible-moves/{x}/{y}/{gameId}")]
        public IActionResult GetPossibleMoves(int x, int y, string gameId)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
            {
                return BadRequest("Invalid position: x and y must be between 0 and 7.");
            }

            var position = new Position(x, y);
            var possibleMoves = _chessService.GetPossibleMoves(gameId,position);
            

            if (possibleMoves == null || possibleMoves.PossibleMoves.Count == 0)
            {
                if(possibleMoves?.PieceType == "None") return NotFound("No piece found at the given position.");
                else return NotFound($"No possible moves found for the {possibleMoves?.PieceType}.");
            }

            return Ok(possibleMoves);
        }
    }
}
