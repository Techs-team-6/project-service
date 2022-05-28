using Microsoft.EntityFrameworkCore;
using ProjectService.WebApi.Entities;

namespace ProjectService.WebApi.Database;

public sealed class ProjectDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectBuild> Builds { get; set; } = null!;

    public ProjectDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=projectsdb;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasMany<ProjectBuild>()
            .WithOne()
            .HasForeignKey(b => b.ProjectId);
    }
}