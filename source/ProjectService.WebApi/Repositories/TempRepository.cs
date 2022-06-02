using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Interfaces;

namespace ProjectService.WebApi.Repositories;

public class TempRepository : ITempRepository
{
    private readonly string _folder;

    public TempRepository(string folder)
    {
        _folder = folder ?? throw new ArgumentException("Null string");
        
        if (!Directory.Exists(Path.Combine(folder)))
            throw new ArgumentException("Directory does not exists");
    }

    public string GetTempFolder(Project project)
    {
        string path = Path.Combine(_folder, project.Id.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public string DeleteFolder(Project project)
    {
        string path = Path.Combine(_folder, project.Id.ToString());
        if (Directory.Exists(path))
        {
            Directory.Delete(path);
        }

        return path;
    }

    public void Clean()
    {
        string path = Path.Combine(_folder);
        if (!Directory.Exists(path)) return;
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            Directory.Delete(directory);
        }
    }
}