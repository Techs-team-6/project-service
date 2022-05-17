namespace ProjectService.Core.Models;

public record ProjectCreateDto(string GithubToken, Uri GithubRepoUri, List<string> Users, string Language);