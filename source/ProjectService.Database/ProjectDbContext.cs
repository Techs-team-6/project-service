using Microsoft.EntityFrameworkCore;
using ProjectService.Shared.Entities;

namespace ProjectService.Database;

public sealed class ProjectDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectBuild> Builds { get; set; } = null!;

    public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>().HasKey(p => p.Id);
        modelBuilder.Entity<ProjectBuild>().HasKey(p => new {p.Id, p.ProjectId});
    }
}
