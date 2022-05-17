namespace ProjectService.Core.Entities;

public record LocalRepository(string Path, Guid ProjectId, string CommitId);