namespace KingsCup.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string UserCode { get; set; }
        public required string Nickname { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
