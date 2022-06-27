using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Fluent;
using ProjectService.Core.Interfaces;
using ProjectService.Database;
using ProjectService.Shared.Entities;

namespace ProjectService.Core.Services;

public class ProjectBuildService : IProjectBuildService
{
    private readonly IGithubService _githubService;
    private readonly ITempRepository _tempRepository;
    private readonly IBuilder _builder;
    private readonly IRepository _repository;
    private readonly ProjectDbContext _context;
    private readonly IArchiver _archiver;

    private readonly Logger _logger;

    public ProjectBuildService(
        IGithubService githubService,
        ITempRepository tempRepository,
        IBuilder builder,
        IRepository repository,
        ProjectDbContext context, IArchiver archiver,
        Logger logger)
    {
        _githubService = githubService;
        _tempRepository = tempRepository;
        _builder = builder;
        _repository = repository;
        _context = context;
        _archiver = archiver;
        _logger = logger;
    }

    public async Task<ProjectBuild> CreateBuildAsync(Project project)
    {
        // clone project into temporary folder
        string tempFolderPath = _tempRepository.GetTempFolder(project);
        _githubService.CloneRepository(tempFolderPath, project);

        // build and compress
        string buildFolderPath = await _builder.Build(tempFolderPath, project.BuildString);

        // save compressed in repository
        Guid storageId;
        await using (Stream fs = _archiver.CompressStream(buildFolderPath))
        {
            storageId = _repository.SaveStream(fs);
        }
        
        
        Directory.Delete(buildFolderPath, recursive: true);

        int newBuildId = GetLastBuildId(project) + 1;

        _logger.Log(LogLevel.Info, "New build of {0} created", project.Name);
        return new ProjectBuild(newBuildId, storageId, project.Id);
    }

    public Stream GetBuild(ProjectBuild build)
    {
        return _repository.GetStream(build.StorageId);
    }

    public async Task DeleteAllBuildsAsync(Guid projectId)
    {
        List<ProjectBuild> builds = await _context.Builds
            .Where(build => build.ProjectId == projectId)
            .ToListAsync();

        foreach (ProjectBuild build in builds)
        {
            _repository.Delete(build.StorageId);
            _logger.Log(LogLevel.Info, "Build of {0} with id: {1} was deleted!", projectId, build.Id);
        }
        
        _logger.Log(LogLevel.Info, "All builds of {0} deleted!", projectId);
        
        _context.RemoveRange(builds);
        await _context.SaveChangesAsync();
    }

    private int GetLastBuildId(Project project, int defaultValue = 0)
    {
        ProjectBuild? lastBuild = _context.Builds
            .Where(build => build.ProjectId == project.Id)
            .OrderByDescending(x => x.Id).FirstOrDefault();

        return lastBuild?.Id ?? defaultValue;
    }
}