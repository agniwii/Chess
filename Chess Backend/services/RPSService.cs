using Chess_Backend.DTOs;
using Chess_Backend.Services.Interfaces;

namespace Chess_Backend.Services
{
    public class RPSService : IRPSService
    {
        private readonly Dictionary<string, Dictionary<string, RockPaperScissorChoice>> _rpsChoices = new Dictionary<string, Dictionary<string,  RockPaperScissorChoice>>();

        public async Task<RPSResponse> SubmitChoice(RPSRequest request)
        {
            if(!_rpsChoices.ContainsKey(request.GameId))
            {
                _rpsChoices[request.GameId] = new Dictionary<string,  RockPaperScissorChoice>();
            }
            _rpsChoices[request.GameId][request.PlayerId] = request.Choice;
            var player = _rpsChoices[request.GameId].Keys.ToList();

            if(player.Count == 2)
            {
                var player1Choice = _rpsChoices[request.GameId][player[0]];
                var player2Choice = _rpsChoices[request.GameId][player[1]];

                var result = await DetermineRPSWinner(player1Choice, player2Choice);

                result.Winner = result.IsDraw ? null : player[result.Winner == player[0] ? 0 : 1] ;
                result.Loser = result.IsDraw ? null : player[result.Winner == player[0] ? 1 : 0] ;
                return result;
            }
            return new RPSResponse {IsDraw = false, Message = "Waiting for other player to make a choice."};
        }

        public async Task<RPSResponse> DetermineRPSWinner(RockPaperScissorChoice choice1, RockPaperScissorChoice choice2)
        {
            await Task.Delay(10);
            if (choice1 == choice2)
            {
                return new RPSResponse { IsDraw = true, Message = "It's a draw!" };
            }
            if ((choice1 == RockPaperScissorChoice.Rock && choice2 == RockPaperScissorChoice.Scissor) || choice1 == RockPaperScissorChoice.Paper && choice2 == RockPaperScissorChoice.Rock || choice1 == RockPaperScissorChoice.Scissor && choice2 == RockPaperScissorChoice.Paper)
            {
                return new RPSResponse { IsDraw = false, Message = $"{choice1} beats {choice2}!" };
            }
            else
            {
                return new RPSResponse { IsDraw = false, Message = $"{choice2} beats {choice1}!" };
            }
        }
    }
}