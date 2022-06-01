using ProjectService.WebApi.Enums;

namespace ProjectService.WebApi.Models;

public record ProjectCreateDto(Guid Id, string GithubToken, string RepositoryName, bool Private, string License, string OwnerName, Language Language, ProjectTemplate Template);