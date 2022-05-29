using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectService.WebApi.Database;
using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Exceptions;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Services;

public class ProjectService : IProjectService
{
    private readonly ProjectDbContext _context;
    private readonly IGithubService _githubService;
    private readonly IProjectBuildService _buildService;

    public ProjectService(
        ProjectDbContext context,
        IGithubService githubService,
        IProjectBuildService buildService)
    {
        _context = context;
        _githubService = githubService;
        _buildService = buildService;
    }

    public Uri AddProject(ProjectCreateDto project)
    {
        if (_context.Projects.Find(project.Id) != null)
        { 
            throw new EntityAlreadyExistsException<Project>(project.Id);
        }
        
        Project createdProject = _githubService.CreateProject(project);

        EntityEntry<Project> entry = _context.Projects.Add(createdProject);
        _context.SaveChanges();
        
        return entry.Entity.Uri;
    }

    public ProjectBuild CreateVersion(Guid projectId)
    {
        Project? project = _context.Projects.Find(projectId);
        if (project == null)
        {
            throw new EntityNotFoundException<Project>(projectId);
        }

        ProjectBuild build = _buildService.CreateBuild(project);
        if (_context.Builds.Find(build.Id, build.ProjectId) != null)
        {
            throw new EntityAlreadyExistsException<Project>(projectId);
        }

        EntityEntry<ProjectBuild> entry = _context.Builds.Add(build);
        _context.SaveChanges();
        return entry.Entity;
    }

    public MemoryStream GetProjectVersionArchive(Guid projectId, int buildId)
    {
        ProjectBuild? build = _context.Builds.Find(buildId, projectId);
        if (build == null)
        {
            throw new EntityNotFoundException<ProjectBuild>(buildId, projectId);
        }

        return _buildService.GetBuild(build);
    }
}