using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GameOfStuff.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Game>()
                .HasIndex(x => x.GameID)
                .IsUnique();
            builder.Entity<Player>()
                .HasKey(x => new { x.PlayerID, x.GameID });
            builder.Entity<Game>()
                .HasMany(x => x.Players)
                .WithOne(x => x.Game)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
    }

    public class Game
    {
        [RegularExpression(@"^\S*$", ErrorMessage = "No spaces allowed")]
        public string GameID { get; set; }
        public string Question { get; set; }
        public string Password { get; set; }
        public List<Player> Players { get; } = new List<Player>();
    }

    public class Player
    {
        public string PlayerID { get; set; }
        public string Answer { get; set; }
        public bool IsOut { get; set; }
        public string GameID { get; set; }
        public Game Game { get; set; }
    }
}
