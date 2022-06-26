using System.Security.Authentication;
using System.Text;
using LibGit2Sharp;
using Octokit;
using Octokit.Internal;
using ProjectService.Core.Interfaces;
using ProjectService.Shared.Models;
using Project = ProjectService.Shared.Entities.Project;

namespace ProjectService.Core.Services;

public class GithubService : IGithubService
{
    private readonly ITempRepository _tempRepository;
    private readonly IProjectCreator _creator;
    private readonly IConfigurationWrapper _configuration;
    private readonly ITemplateService _templateService;
    private const string AppName = "ProjectService";

    private static readonly Identity Identity = new("ProjectService", "projectService@noreplay.com");

    public GithubService(ITempRepository tempRepository, IProjectCreator creator, IConfigurationWrapper configuration, ITemplateService templateService)
    {
        _tempRepository = tempRepository;
        _creator = creator;
        _configuration = configuration;
        _templateService = templateService;
    }

    public async Task<Project> CreateProjectAsync(ProjectCreateDto dto, Guid templateId)
    {
        Octokit.Repository? repository = await CreateEmptyRepository(dto);
        if (repository is null)
            throw new ArgumentException("Repository can not be created fore some magic reason");

        var project = new Project(dto.Id, new Uri(repository.CloneUrl), dto.RepositoryName, string.Empty);
        string folder = _tempRepository.GetTempFolder(project);
        CloneRepository(folder, project);
        await _creator.CreateAsync(folder, dto.RepositoryName, templateId);
        string buildString = 
            templateId != default 
            ? $"dotnet build \"{Path.Combine(project.Name, project.Name)}.csproj\" -c Release" 
            : _templateService.GetTemplateBuildString(templateId);
        
        project.BuildString = buildString;
        string workflowContent = CreateWorkflow(project);
        Directory.CreateDirectory(Path.Combine(folder, ".github"));
        Directory.CreateDirectory(Path.Combine(folder, ".github/workflows"));
        await File.WriteAllTextAsync(Path.Combine(folder, ".github/workflows/publish.yml"), workflowContent);
        StageChanges(folder);
        CommitChanges(folder, "init project");
        PushChanges(folder);
        return project;
    }

    public void CloneRepository(string path, Project project)
    {
        if (!Directory.Exists(path))
            throw new ArgumentException("Directory does not exists");

        LibGit2Sharp.Credentials credentials = GetLibGit2SharpCredentials();

        if (Directory.GetFiles(path).Length == 0)
        {
            var cloneOptions = new CloneOptions {Checkout = true, CredentialsProvider = (_, _, _) => credentials};
            LibGit2Sharp.Repository.Clone(project.Uri.ToString(), path, cloneOptions);
            return;
        }

        if (!LibGit2Sharp.Repository.IsValid(path))
            throw new ArgumentException($"Folder {path} is not empty, but there are no repository found");

        var repo = new LibGit2Sharp.Repository(path);
        if (repo.Network.Remotes.Select(x => new Uri(x.Url)).All(x => !x.Equals(project.Uri)))
            throw new ArgumentException(
                $"There is a repository in {path} found, but none of its remotes matcher project uri");

        var pullOptions = new PullOptions()
        {
            MergeOptions = new MergeOptions()
            {
                FastForwardStrategy = FastForwardStrategy.Default
            },
            FetchOptions = new FetchOptions()
            {
                CredentialsProvider = (_, _, _) => credentials
            }
        };

        Commands.Pull(repo,
            new LibGit2Sharp.Signature(Identity, DateTimeOffset.Now),
            pullOptions);
    }

    private async Task<Octokit.Repository?> CreateEmptyRepository(ProjectCreateDto dto)
    {
        ICredentialStore store = new InMemoryCredentialStore(new Octokit.Credentials(_configuration.GithubToken));
        var client = new GitHubClient(new ProductHeaderValue(AppName), store);
        var repository = new NewRepository(dto.RepositoryName)
        {
            AutoInit = true,
            Description = "",
            Private = dto.Private
        };
        
        if (string.IsNullOrEmpty(_configuration.GithubOrganization))
            return await client.Repository.Create(repository);
        
        return await client.Repository.Create(_configuration.GithubOrganization, repository);
    }

    private LibGit2Sharp.Credentials GetLibGit2SharpCredentials()
    {
        ICredentialStore store = new InMemoryCredentialStore(new Octokit.Credentials(_configuration.GithubToken));
        var client = new GitHubClient(new ProductHeaderValue(AppName), store);

        if (client is null) throw new AuthenticationException("Invalid token");

        LibGit2Sharp.Credentials credentials = new UsernamePasswordCredentials()
        {
            Username = _configuration.GithubUsername,
            Password = _configuration.GithubToken
        };

        return credentials;
    }

    private string CreateWorkflow(Project project)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("on:");
        stringBuilder.AppendLine("  push:");
        stringBuilder.AppendLine("      branches:");
        stringBuilder.AppendLine("      - main");
        stringBuilder.AppendLine("jobs:");
        stringBuilder.AppendLine("  deploy:");
        stringBuilder.AppendLine("      runs-on: ubuntu-latest");
        stringBuilder.AppendLine("      steps:");
        stringBuilder.AppendLine("      - name: Deploy");
        stringBuilder.AppendLine("        uses: fjogeleit/http-request-action@v1.9.1");
        stringBuilder.AppendLine("        with:");
        stringBuilder.AppendLine($"          url: {_configuration.ProjectServiceAddress}/api/v1/projects/{project.Id}/builds/create");
        stringBuilder.AppendLine("          method: POST");
        return stringBuilder.ToString();
    }

    private void PushChanges(string path, string branchName = "main")
    {
        LibGit2Sharp.Credentials cred = GetLibGit2SharpCredentials();
        var repo = new LibGit2Sharp.Repository(path);
        LibGit2Sharp.Branch? branch = repo.Branches[branchName];
        var options = new PushOptions()
        {
            CredentialsProvider = (_, _, _) => cred
        };

        repo.Network.Push(branch, options);
    }

    private static void StageChanges(string path)
    {
        var repo = new LibGit2Sharp.Repository(path);
        RepositoryStatus status = repo.RetrieveStatus();
        var filePaths = status.Added.Select(mods => mods.FilePath).ToList();
        filePaths.AddRange(status.Modified.Select(mods => mods.FilePath).ToList());
        filePaths.AddRange(status.Untracked.Select(mods => mods.FilePath).ToList());
        Commands.Stage(repo, filePaths);
    }

    private static void CommitChanges(string path, string message)
    {
        var repo = new LibGit2Sharp.Repository(path);
        repo.Commit(message,
            new LibGit2Sharp.Signature(Identity, DateTimeOffset.Now),
            new LibGit2Sharp.Signature(Identity, DateTimeOffset.Now));
    }
}