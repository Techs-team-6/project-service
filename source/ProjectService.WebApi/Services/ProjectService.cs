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
    private readonly IConfigurationWrapper _configuration;
    private readonly IBuildNotifier _buildNotifier;

    public ProjectService(
        ProjectDbContext context,
        IGithubService githubService,
        IProjectBuildService buildService, 
        IConfigurationWrapper configuration,
        IBuildNotifier buildNotifier)
    {
        _context = context;
        _githubService = githubService;
        _buildService = buildService;
        _configuration = configuration;
        _buildNotifier = buildNotifier;
    }

    public async Task<Uri> AddProjectAsync(ProjectCreateDto project)
    {
        if (await _context.Projects.FindAsync(project.Id) != null)
        { 
            throw new EntityAlreadyExistsException<Project>(project.Id);
        }
        
        Project createdProject = await _githubService.CreateProjectAsync(project);

        EntityEntry<Project> entry = await _context.Projects.AddAsync(createdProject);
        await _context.SaveChangesAsync();
        
        return entry.Entity.Uri;
    }

    public async Task<ProjectBuild> CreateVersionAsync(Guid projectId)
    {
        Project? project = _context.Projects.Find(projectId);
        if (project == null)
        {
            throw new EntityNotFoundException<Project>(projectId);
        }

        ProjectBuild build = await _buildService.CreateBuildAsync(project);
        if (_context.Builds.Find(build.Id, build.ProjectId) != null)
        {
            throw new EntityAlreadyExistsException<Project>(projectId);
        }

        EntityEntry<ProjectBuild> entry = _context.Builds.Add(build);
        _context.SaveChanges();

        await _buildNotifier.NotifyOnBuildAsync(entry.Entity);
        
        return entry.Entity;
    }

    public Stream GetProjectVersionArchive(Guid projectId, int buildId)
    {
        ProjectBuild? build = _context.Builds.Find(buildId, projectId);
        if (build == null)
        {
            throw new EntityNotFoundException<ProjectBuild>(buildId, projectId);
        }

        return _buildService.GetBuild(build);
    }

    public Stream GetProjectVersionArchive(Guid storageId)
    {
        ProjectBuild? build = _context.Builds.First(x => x.StorageId == storageId);
        if (build == null)
        {
            throw new EntityNotFoundException<ProjectBuild>(storageId);
        }

        return _buildService.GetBuild(build);
    }

    public string? UpdateBuildString(Guid projectId, string newBuildString)
    {
        if (newBuildString == null) throw new ArgumentNullException(nameof(newBuildString));
        
        Project? project = _context.Projects.Find(projectId);
        if (project is null)
            return null;

        project.BuildString = newBuildString;
        _context.Update(project);

        return newBuildString;
    }

    public GitInfo GetGitInfo()
    {
        return new GitInfo(_configuration.GithubUsername, _configuration.GithubOrganization);
    }
}