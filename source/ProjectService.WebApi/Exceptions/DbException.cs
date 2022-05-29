namespace ProjectService.WebApi.Exceptions;

public class DbException : Exception
{
    public DbException(string message) : base(message)
    {
    }

    public DbException(string message, Exception inner) : base(message, inner)
    {
    }
}