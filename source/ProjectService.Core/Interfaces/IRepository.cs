namespace ProjectService.Core.Interfaces;

public interface IRepository
{
    Guid Save(object entity);
    object Get(Guid id);
}