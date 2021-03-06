using Microsoft.EntityFrameworkCore;
using System;
using WebApiFootballTournament.Entities;

namespace WebApiFootballTournament.DbContexts
{
    public class FootballTournamentContext : DbContext
    {
        public FootballTournamentContext(DbContextOptions<FootballTournamentContext> options)
            : base(options)
        {
        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasData(
                new Team()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    Name = "Team1",
                    Description = "Description1",
                    GroupId = Guid.Parse("5c7e7eea-04fc-4935-8b18-d9c88da3b828")
                },
                new Team()
                {
                    Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                    Name = "Team2",
                    Description = "Description2"
                }
            );

            modelBuilder.Entity<Group>().HasData(
                new Group()
                {
                    Id = Guid.Parse("5c7e7eea-04fc-4935-8b18-d9c88da3b828"),
                    Name = "A"
                },
                new Group()
                {
                    Id = Guid.Parse("2107ce76-80ac-4041-a80a-3736cce78f1b"),
                    Name = "B"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
