using ProjectService.WebApi.Entities;

namespace ProjectService.WebApi.Interfaces;

public interface ITempRepository
{
    string GetTempFolder(Project project);
    string DeleteFolder(Project project);
    void Clean();
}