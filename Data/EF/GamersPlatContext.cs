using System;
using Microsoft.EntityFrameworkCore;

namespace GamersPlat.Data.EF
{
    public class GamersPlatContext : DbContext
    {
        public GamersPlatContext(DbContextOptions<GamersPlatContext> options) : base(options)
        {
        }
        // Define your DbSets here, for example:
        // public DbSet<User> Users { get; set; }
        // public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure your database connection here, for example:
            // optionsBuilder.UseSqlServer("YourConnectionString");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity relationships and constraints here, for example:
            // modelBuilder.Entity<User>().HasMany(u => u.Games).WithOne(g => g.User);
        }
    }
}