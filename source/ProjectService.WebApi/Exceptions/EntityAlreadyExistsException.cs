using System.Text;

namespace ProjectService.WebApi.Exceptions;

public class EntityAlreadyExistsException<T> : Exception
where T : class
{
    public EntityAlreadyExistsException(object primaryKey, params object[] primaryKeys)
    :base(BuildMessage(primaryKey, primaryKeys))
    {
    }

    private static string BuildMessage(object primaryKey, params object[] primaryKeys)
    {
        var sb = new StringBuilder($"Entity of type {typeof(T)} with primary key(s) {{{primaryKey}");
        foreach (object key in primaryKeys)
        {
            sb.Append($", {key}");
        }

        sb.Append("} has already been added.");
        return sb.ToString();
    }
}