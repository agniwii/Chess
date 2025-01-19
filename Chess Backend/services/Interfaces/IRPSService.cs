using Chess_Backend.DTOs;
using Chess_Backend.Models;


namespace Chess_Backend.Services.Interfaces
{
    public interface IRPSService
    {
        Task<RPSResponse> SubmitChoice(RPSRequest request);
    }
}
