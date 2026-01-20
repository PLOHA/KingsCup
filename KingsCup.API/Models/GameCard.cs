using System.ComponentModel.DataAnnotations;

namespace KingsCup.API.Models
{
    public class GameCard
    {
        [Key]
        public int Id { get; set; }

        public int RoomId { get; set; }

        public required string Suit { get; set; }
        public required string Rank { get; set; }
        
        public bool IsDrawn { get; set; } = false;
        public string? DrawnByUserCode { get; set; }

        public int OrderIndex { get; set; }
    }
}
