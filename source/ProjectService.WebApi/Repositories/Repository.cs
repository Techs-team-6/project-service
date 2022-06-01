using ProjectService.WebApi.Interfaces;

namespace ProjectService.WebApi.Repositories;

public class Repository : IRepository
{
    private readonly string _folder;

    public Repository(string folder)
    {
        _folder = folder ?? throw new ArgumentException("Null string");
        
        if (!Directory.Exists(Path.Combine(folder)))
            throw new ArgumentException("Directory does not exists");
    }

    public Guid SaveStream(Stream entity)
    {
        var id = Guid.NewGuid();
        
        var fileStream = new FileStream(Path.Combine(_folder, id.ToString()), FileMode.CreateNew);
        entity.CopyTo(fileStream);
        fileStream.Close();

        return id;
    }

    public Stream GetStream(Guid id)
    {
        if (!Directory.Exists(Path.Combine(_folder, id.ToString())))
            throw new ArgumentException("this id does not exists");
        
        return new FileStream(Path.Combine(_folder, id.ToString()), FileMode.Open);
    }
}