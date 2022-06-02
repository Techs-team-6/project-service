namespace ProjectService.WebApi.Interfaces;

public interface IConfigurationWrapper
{
    string GithubUsername { get; }
    string GithubToken { get; }
    string? GithubOrganization { get; }
    string ProjectServiceAddress { get; }
    string ServerAddress { get; }
    
}