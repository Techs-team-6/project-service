using ProjectService.WebApi.Enums;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectCreator
{
    public string Create(string path);
}