using System.Text;

namespace ProjectService.WebApi.Exceptions;

public class EntityNotFoundException<T> : ApplicationException
    where T : class
{
    public EntityNotFoundException(object primaryKey, params object[] primaryKeys) 
        : base(BuildMessage(primaryKey, primaryKeys))
    {
    }
    
    private static string BuildMessage(object primaryKey, params object[] primaryKeys)
    {
        var sb = new StringBuilder($"Entity of type {typeof(T)} with primary key(s) {{{primaryKey}");
        foreach (object key in primaryKeys)
        {
            sb.Append($", {key}");
        }

        sb.Append("} not found.");
        return sb.ToString();
    }
}