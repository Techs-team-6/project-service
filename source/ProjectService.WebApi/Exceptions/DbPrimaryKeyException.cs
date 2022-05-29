namespace ProjectService.WebApi.Exceptions;

public class DbPrimaryKeyException : Exception
{
    public DbPrimaryKeyException(string message) : base(message)
    {
    }

    public DbPrimaryKeyException(string message, Exception inner) : base(message, inner)
    {
    }
}