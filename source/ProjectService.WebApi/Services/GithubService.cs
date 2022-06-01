using System.Security.Authentication;
using LibGit2Sharp;
using Octokit;
using ProjectService.WebApi.Interfaces;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Services;

public class GithubService : IGithubService
{
    private readonly ITempRepository _tempRepository;
    private readonly IProjectCreator _creator;

    public GithubService(ITempRepository tempRepository, IProjectCreator creator)
    {
        _tempRepository = tempRepository;
        _creator = creator;
    }

    public async Task<Entities.Project> CreateProject(ProjectCreateDto dto)
    {
        Octokit.Repository? repository = await GenerateEmptyRepository(dto);
        if (repository is null)
            throw new ArgumentException("Repository can not be created fore some magic reason");

        var project = new Entities.Project(dto.Id, new Uri(repository.Url), dto.RepositoryName, dto.GithubToken);
        string folder = _tempRepository.GetTempFolder(project);
        CloneRepository(folder, project);
        _creator.Create(folder, dto.Language, dto.Template);
        return project;
    }

    public void CloneRepository(string path, Entities.Project project)
    {
        if (!Directory.Exists(path))
            throw new ArgumentException("Directory does not exists");

        LibGit2Sharp.Credentials credentials = GetLibGit2SharpCredentials(project.GithubToken);

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
        
        LibGit2Sharp.Commands.Pull(repo, new LibGit2Sharp.Signature("ProjectService", "projectService@noreplay.com", DateTimeOffset.Now), pullOptions);
    }
    
    private async Task<Octokit.Repository?> GenerateEmptyRepository(ProjectCreateDto dto)
    {
        var basicAuth = new Octokit.Credentials(dto.GithubToken);
        var client = new GitHubClient(new ProductHeaderValue(dto.RepositoryName))
        {
            Credentials = basicAuth
        };
        var repository = new NewRepository(dto.RepositoryName)
        {
            AutoInit = false,
            Description = "",
            LicenseTemplate = dto.License,
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
}