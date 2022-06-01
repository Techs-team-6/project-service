namespace ProjectService.WebApi.Interfaces;

public interface IRepository
{
    Guid SaveStream(Stream entity);
    MemoryStream GetStream(Guid id);
}