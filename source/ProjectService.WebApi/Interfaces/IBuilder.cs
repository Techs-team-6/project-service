using ProjectService.WebApi.Enums;

namespace ProjectService.WebApi.Interfaces;

public interface IBuilder
{
    Language Language { get; }
    string Build(string path);
}