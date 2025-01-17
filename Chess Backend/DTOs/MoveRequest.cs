using System.ComponentModel.DataAnnotations;
using Chess_Backend.Models;

namespace Chess_Backend.DTOs
{
    public class MoveRequest
    {
        [Required]
        public required string GameId { get; set; }
        [Required]
        public required string PlayerId { get; set; }

        [Required]
        public required Position From { get; set; }
        [Required]
        public required Position To { get; set; }
    }
}
