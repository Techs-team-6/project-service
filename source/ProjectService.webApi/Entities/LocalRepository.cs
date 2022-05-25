namespace ProjectService.WebApi.Entities;

public record LocalRepository(string Path, Guid ProjectId, string CommitId);