namespace ProjectService.Core.Interfaces;

public interface IBuilder
{
    Task<string> Build(string path, string buildString);
}