using KingsCup.API.Models;
using Microsoft.EntityFrameworkCore;

namespace KingsCup.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomPlayer> RoomPlayers { get; set; }
        public DbSet<GameCard> GameCards { get; set; }
    }
}
