using System.IO.Compression;
using ProjectService.WebApi.Database;
using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Interfaces;

namespace ProjectService.WebApi.Services;

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

    public ProjectBuild CreateBuild(Project project)
    {
        // clone project into temporary folder
        string tempFolderPath = _tempRepository.GetTempFolder(project);
        _githubService.CloneRepository(tempFolderPath, project);

        // build and compress
        string buildFolderPath = _builder.Build(PathToClonedProject(tempFolderPath, project.Name));
        string fullBuildZipName = CreateBuildArchive(buildFolderPath, tempFolderPath, project.Name);

        // save compressed in repository
        Guid storageId =  _repository.SaveStream(GetMemoryStream(fullBuildZipName));
        int newBuildId = GetLastBuildId(project) + 1;
        
        _tempRepository.DeleteFolder(project);

        return new ProjectBuild(newBuildId, storageId, project.Id);
    }

    public MemoryStream GetBuild(ProjectBuild build)
    {
        return _repository.GetStream(build.StorageId);
    }

    private static string PathToClonedProject(string path, string projectName)
    {
        return Path.Combine(path, projectName);
    }

    private static string CreateBuildArchive(
        string buildFolderPath,
        string tempFolderPath,
        string projectName)
    {
        var memoryStream = new MemoryStream();
        var zipArchive = new ZipArchive(memoryStream);

        string fullBuildZipName = FullBuildZipName(tempFolderPath, projectName);
        ZipFile.CreateFromDirectory(
            buildFolderPath,
            fullBuildZipName,
            CompressionLevel.Optimal,
            includeBaseDirectory: true);

        return fullBuildZipName;
    }

    private static string FullBuildZipName(string pathToZip, string projectName)
    {
        return Path.Combine(pathToZip, $"{projectName}.zip");
    }

    private static MemoryStream GetMemoryStream(string file)
    {
        var memoryStream = new MemoryStream();
        using FileStream fileStream = File.OpenRead(file);
        fileStream.CopyTo(memoryStream);

        return memoryStream;
    }

    private int GetLastBuildId(Project project, int defaultValue = 0)
    {
        ProjectBuild? lastBuild = _context.Builds
            .Where(build => build.ProjectId == project.Id)
            .MaxBy(build => build.Id);

        return lastBuild?.Id ?? defaultValue;
    }
}