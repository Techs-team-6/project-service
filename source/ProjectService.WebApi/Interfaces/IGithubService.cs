using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Interfaces;

public interface IGithubService
{
    Project CreateProject(ProjectCreateDto dto);
    void CloneRepository(string path, Project project);
}