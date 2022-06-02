using System.Security.Authentication;
using System.Text;
using LibGit2Sharp;
using Octokit;
using Octokit.Internal;
using ProjectService.WebApi.Enums;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;
using Branch = LibGit2Sharp.Branch;
using Credentials = LibGit2Sharp.Credentials;

namespace ProjectService.WebApi.Services;

public class GithubService : IGithubService
{
    private readonly ITempRepository _tempRepository;
    private readonly IProjectCreator _creator;
    private readonly GitInfo _gitInfo;
    private readonly IConfiguration _configuration;

    private static readonly LibGit2Sharp.Identity Identity =
        new Identity("ProjectService", "projectService@noreplay.com");

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

        var project = new Entities.Project(dto.Id, new Uri(repository.CloneUrl), dto.RepositoryName, string.Empty);
        string folder = _tempRepository.GetTempFolder(project);
        CloneRepository(folder, project);
        string csprojPath = await _creator.Create(folder, dto.RepositoryName);
        string buildString = "dotnet build " + csprojPath;
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

    public void CloneRepository(string path, Entities.Project project)
    {
        if (!Directory.Exists(path))
            throw new ArgumentException("Directory does not exists");

        LibGit2Sharp.Credentials credentials = GetLibGit2SharpCredentials();

        if (Directory.GetFiles(path).Length == 0)
        {
            var cloneOptions = new CloneOptions { Checkout = true, CredentialsProvider = (_url, _user, _cred) => credentials};
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
            new LibGit2Sharp.Signature(Identity, DateTimeOffset.Now),
            pullOptions);
    }
    
    private async Task<Octokit.Repository?> GenerateEmptyRepository(ProjectCreateDto dto)
    {
        ICredentialStore store = new InMemoryCredentialStore(new Octokit.Credentials(_gitInfo.GithubToken));
        var client = new GitHubClient(new ProductHeaderValue(dto.RepositoryName), store);
        var repository = new NewRepository(dto.RepositoryName)
        {
            AutoInit = true,
            Description = "",
            Private = dto.Private
        };
        Octokit.Repository? context = await client.Repository.Create(repository);
        return context;
    }

    private LibGit2Sharp.Credentials GetLibGit2SharpCredentials()
    {
        ICredentialStore store = new InMemoryCredentialStore(new Octokit.Credentials(_gitInfo.GithubToken));
        var client = new GitHubClient(new ProductHeaderValue("cool-app"), store);

        if (client is null)
        {
            throw new AuthenticationException("Invalid token");
        }

        LibGit2Sharp.Credentials credentials = new UsernamePasswordCredentials()
        {
            Username = _gitInfo.Login,
            Password = _gitInfo.GithubToken
            
        };

        return credentials;
    }

    private string CreateWorkflow(Entities.Project project)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("on:");
        stringBuilder.AppendLine("  push:");
        stringBuilder.AppendLine("   branches:");
        stringBuilder.AppendLine("    - master");
        stringBuilder.AppendLine("jobs:");
        stringBuilder.AppendLine("  deploy:");
        stringBuilder.AppendLine("   runs-on: ubuntu-latest");
        stringBuilder.AppendLine("   steps:");
        stringBuilder.AppendLine("   - name:Deploy");
        stringBuilder.AppendLine("     uses: fjogeleit/http-request-action@v1");
        stringBuilder.AppendLine("     with:");
        stringBuilder.AppendLine($"      url: https://{_configuration["ProjectServiceAddress"]}/api/v1/projects/{project.Id}/builds/create");
        stringBuilder.AppendLine("       method: POST");
        return stringBuilder.ToString();
    }
    
    private void StageChanges(string path) {
        try {
            var repo = new LibGit2Sharp.Repository(path);
            RepositoryStatus status = repo.RetrieveStatus();
            var filePaths = status.Added.Select(mods => mods.FilePath).ToList();
            filePaths.AddRange(status.Modified.Select(mods => mods.FilePath).ToList());
            filePaths.AddRange(status.Untracked.Select(mods => mods.FilePath).ToList());
            LibGit2Sharp.Commands.Stage(repo, filePaths);
        }
        catch (Exception ex) {
            Console.WriteLine("Exception:RepoActions:StageChanges " + ex.Message);
        }
    }

    private void CommitChanges(string path, string message)
    {
        try {
            var repo = new LibGit2Sharp.Repository(path);
            repo.Commit(message,
                new LibGit2Sharp.Signature(Identity, DateTimeOffset.Now),
                new LibGit2Sharp.Signature(Identity, DateTimeOffset.Now));
        }
        catch (Exception e) {
            Console.WriteLine("Exception:RepoActions:CommitChanges " + e.Message);
        }
    }

    private void PushChanges(string path, string branchName = "main") {
        Credentials cred = GetLibGit2SharpCredentials();
        var repo = new LibGit2Sharp.Repository(path);
        try {
            Branch? branch = repo.Branches[branchName];
            var options = new PushOptions()
            {
                CredentialsProvider = (_, _, _) => cred,
                
            };
            
            repo.Network.Push(branch, options);
        }
        catch (Exception e) {
            Console.WriteLine("Exception:RepoActions:PushChanges " + e.Message);
        }
    }
}