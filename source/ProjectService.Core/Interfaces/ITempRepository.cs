using ProjectService.Shared.Entities;

namespace ProjectService.Core.Interfaces;

public interface ITempRepository
{
    string GetTempFolder(Project project);
    string DeleteFolder(Project project);
    void Clean();
}