using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
                .HasKey(p => new { p.PlayerID, p.GameID });
            builder.Entity<Game>()
                .HasMany(g => g.Players)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Question> Questions { get; set; }
    }

    public class Game
    {
        [Required]
        [StringLength(30, ErrorMessage = "Name is too long.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "No spaces allowed")]
        public string GameID { get; set; }

        public string Question { get; set; }

        [StringLength(30, ErrorMessage = "Password is too long.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "No spaces allowed")]
        public string Password { get; set; }

        public List<Player> Players { get; } = new List<Player>();
    }

    public class Question
    {
        public int QuestionID { get; set; }
        public string Text { get; set; }
    }

    public class Player
    {
        [Required]
        [StringLength(30, ErrorMessage = "Name is too long.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "No spaces allowed")]
        public string PlayerID { get; set; }

        [StringLength(140, ErrorMessage = "Answer is too long.")]
        public string Answer { get; set; }

        public bool IsOut { get; set; }

        public string GameID { get; set; }
    }
}
