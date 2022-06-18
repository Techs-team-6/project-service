namespace ProjectService.Core.Interfaces;

public interface IRepository
{
    Guid SaveStream(Stream entity);
    Stream GetStream(Guid id);
}