namespace ProjectService.Core.Interfaces;

public interface IRepository<T>
{
    Guid SaveFile(T entity);
    T GetFile(Guid id);
}