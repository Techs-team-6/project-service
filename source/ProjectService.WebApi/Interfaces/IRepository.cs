namespace ProjectService.WebApi.Interfaces;

public interface IRepository
{
    Guid SaveStream(MemoryStream entity);
    MemoryStream GetStream(Guid id);
}