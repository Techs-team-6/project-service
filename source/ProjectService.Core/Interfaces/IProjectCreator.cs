using System.Security.Cryptography.X509Certificates;

namespace ProjectService.Core.Interfaces;

public interface IProjectCreator
{
    Task<string> CreateAsync(string path, string projectName, Guid templateId = default);
}