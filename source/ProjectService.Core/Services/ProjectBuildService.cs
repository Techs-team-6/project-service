using System.IO.Compression;
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

    public ProjectBuildService(
        IGithubService githubService,
        ITempRepository tempRepository,
        IBuilder builder,
        IRepository repository,
        ProjectDbContext context)
    {
        _githubService = githubService;
        _tempRepository = tempRepository;
        _builder = builder;
        _repository = repository;
        _context = context;
    }

    public async Task<ProjectBuild> CreateBuildAsync(Project project)
    {
        // clone project into temporary folder
        string tempFolderPath = _tempRepository.GetTempFolder(project);
        _githubService.CloneRepository(tempFolderPath, project);

        // build and compress
        string buildFolderPath = await _builder.Build(tempFolderPath, project.BuildString);
        string fullBuildZipName = CreateBuildArchive(buildFolderPath, tempFolderPath, project.Name);

        // save compressed in repository
        Guid storageId;
        await using (FileStream fs = File.OpenRead(fullBuildZipName))
        {
            storageId = _repository.SaveStream(fs);
        }

        File.Delete(fullBuildZipName);
        Directory.Delete(buildFolderPath, recursive: true);

        int newBuildId = GetLastBuildId(project) + 1;

        return new ProjectBuild(newBuildId, storageId, project.Id);
    }

    public Stream GetBuild(ProjectBuild build)
    {
        return _repository.GetStream(build.StorageId);
    }

    private static string CreateBuildArchive(
        string buildFolderPath,
        string tempFolderPath,
        string projectName)
    {
        string fullBuildZipName = FullBuildZipName(tempFolderPath, projectName);
        ZipFile.CreateFromDirectory(
            buildFolderPath,
            fullBuildZipName,
            CompressionLevel.Optimal,
            includeBaseDirectory: false);

        return fullBuildZipName;
    }

    private static string FullBuildZipName(string pathToZip, string projectName)
    {
        return Path.Combine(pathToZip, $"{projectName}.zip");
    }

    private int GetLastBuildId(Project project, int defaultValue = 0)
    {
        ProjectBuild? lastBuild = _context.Builds
            .Where(build => build.ProjectId == project.Id)
            .OrderByDescending(x => x.Id).FirstOrDefault();

        return lastBuild?.Id ?? defaultValue;
    }
}