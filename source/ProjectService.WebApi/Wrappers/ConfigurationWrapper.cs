using ProjectService.WebApi.Interfaces;

namespace ProjectService.WebApi.Wrappers;

public class ConfigurationWrapper : IConfigurationWrapper
{
    private readonly IConfiguration _configuration;


    public ConfigurationWrapper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GithubUsername { get => _configuration["GithubUsername"]; }
    public string GithubToken { get => _configuration["GithubToken"]; }
    public string? GithubOrganization { get => _configuration["GithubOrganization"]; }
    public string ProjectServiceAddress { get => _configuration["ProjectServiceApiUrl"]; }
    public string ServerAddress { get => _configuration["ServerAddress"]; }
}   