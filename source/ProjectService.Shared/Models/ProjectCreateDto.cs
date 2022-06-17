namespace ProjectService.Shared.Models;

public record ProjectCreateDto(Guid Id, string RepositoryName, bool Private);