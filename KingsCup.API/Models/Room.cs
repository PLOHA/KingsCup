namespace KingsCup.API.Models
{
    public class Room
    {
        public int Id { get; set; }
        public required string RoomCode { get; set; }
        public string Status { get; set; } = "waiting";
        public DateTime? StartTime { get; set; }
        public string? CurrentTurnUserCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
