using ProjectService.WebApi.Enums;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectCreator
{
    public void Create(string path, Language language, ProjectTemplate projectTemplate);
}