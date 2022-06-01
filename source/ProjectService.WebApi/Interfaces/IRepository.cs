namespace ProjectService.WebApi.Interfaces;

public interface IRepository
{
    Guid SaveStream(Stream entity);
    Stream GetStream(Guid id);
}