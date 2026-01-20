namespace KingsCup.API.Models
{
    public class RoomPlayer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string UserCode { get; set; }
        public required string Nickname { get; set; }
        public int RoomId { get; set; }
        public bool IsReady { get; set; } = false;
        public int TotalSips { get; set; } = 0;
    }
}
