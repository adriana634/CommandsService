using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine("--> Using InMemory database");

            optionsBuilder.UseInMemoryDatabase("CommandsDB");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Platform>()
                .HasMany(platform => platform.Commands)
                .WithOne(command => command.Platform)
                .HasForeignKey(command => command.PlatformId);
                
            modelBuilder
                .Entity<Command>()
                .HasOne(command => command.Platform)
                .WithMany(platform => platform.Commands)
                .HasForeignKey(command => command.PlatformId);

            base.OnModelCreating(modelBuilder);
        }
    }
}