using Microsoft.EntityFrameworkCore;
using ProjectService.WebApi.Entities;

namespace ProjectService.WebApi.Database;

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
        modelBuilder.Entity<Project>()
            .HasMany<ProjectBuild>()
            .WithOne()
            .HasForeignKey(b => b.ProjectId);
    }
}