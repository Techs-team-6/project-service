using System.Security.Authentication;
using System.Text;
using LibGit2Sharp;
using Octokit;
using ProjectService.WebApi.Enums;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Services;

public class GithubService : IGithubService
{
    private readonly ITempRepository _tempRepository;
    private readonly IProjectCreator _creator;
    private readonly GitInfo _gitInfo;
    private readonly IConfiguration _configuration;

    public GithubService(ITempRepository tempRepository, IProjectCreator creator, GitInfo gitInfo, IConfiguration configuration)
    {
        _tempRepository = tempRepository;
        _creator = creator;
        _gitInfo = gitInfo;
        _configuration = configuration;
    }

    public async Task<Entities.Project> CreateProject(ProjectCreateDto dto)
    {
        Octokit.Repository? repository = await GenerateEmptyRepository(dto);
        if (repository is null)
            throw new ArgumentException("Repository can not be created fore some magic reason");

        var project = new Entities.Project(dto.Id, new Uri(repository.Url), dto.RepositoryName, string.Empty);
        string folder = _tempRepository.GetTempFolder(project);
        CloneRepository(folder, project);
        string csprojPath = await _creator.Create(folder, dto.RepositoryName);
        string buildString = "dotnet build " + csprojPath;
        project.BuildString = buildString;
        string workflowContent = CreateWorkflow(project);
        Directory.CreateDirectory(Path.Combine(folder, ".github"));
        Directory.CreateDirectory(Path.Combine(folder, ".github/workflows"));
        await File.WriteAllTextAsync(Path.Combine(folder, ".github/workflows/publish.yml"), workflowContent);
        return project;
    }

    public void CloneRepository(string path, Entities.Project project)
    {
        if (!Directory.Exists(path))
            throw new ArgumentException("Directory does not exists");

        LibGit2Sharp.Credentials credentials = GetLibGit2SharpCredentials(_gitInfo.GithubToken);

        if (Directory.GetFiles(path).Length == 0)
        {
            var cloneOptions = new CloneOptions { Checkout = true, CredentialsProvider = (_, _, _) => credentials};
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

        LibGit2Sharp.Commands.Pull(repo,
            new LibGit2Sharp.Signature("ProjectService", "projectService@noreplay.com", DateTimeOffset.Now),
            pullOptions);
    }
    
    private async Task<Octokit.Repository?> GenerateEmptyRepository(ProjectCreateDto dto)
    {
        var basicAuth = new Octokit.Credentials(_gitInfo.GithubToken);
        var client = new GitHubClient(new ProductHeaderValue(dto.RepositoryName))
        {
            Credentials = basicAuth
        };
        var repository = new NewRepository(dto.RepositoryName)
        {
            AutoInit = false,
            Description = "",
            Private = dto.Private
        };
        Octokit.Repository? context = await client.Repository.Create(repository);
        return context;
    }

    private LibGit2Sharp.Credentials GetLibGit2SharpCredentials(string token)
    {
        var client = new GitHubClient(new ProductHeaderValue(""))
        {
            Credentials = new Octokit.Credentials(token)
        };

        if (client is null)
        {
            throw new AuthenticationException("Invalid token");
        }

        LibGit2Sharp.Credentials credentials = new UsernamePasswordCredentials()
        {
            Username = client.Credentials.Login,
            Password = token
            
        };

        return credentials;
    }

    private string CreateWorkflow(Entities.Project project)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("on:");
        stringBuilder.AppendLine(" push:");
        stringBuilder.AppendLine("  branches:");
        stringBuilder.AppendLine("   - master");
        stringBuilder.AppendLine("  jobs:");
        stringBuilder.AppendLine("   deploy:");
        stringBuilder.AppendLine("    runs-on: ubuntu-latest");
        stringBuilder.AppendLine("    steps:");
        stringBuilder.AppendLine("     - name:Deploy");
        stringBuilder.AppendLine("       uses: fjogeleit/http-request-action@v1");
        stringBuilder.AppendLine("       with:");
        stringBuilder.AppendLine($"       url: https://{_configuration["ProjectServiceAddress"]}/api/v1/projects/{project.Id}/builds/create");
        stringBuilder.AppendLine("        method: POST");
        return stringBuilder.ToString();
    }
}