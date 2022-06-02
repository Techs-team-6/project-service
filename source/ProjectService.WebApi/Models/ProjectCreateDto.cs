namespace ProjectService.WebApi.Models;

public record ProjectCreateDto(Guid Id, string RepositoryName, bool Private);