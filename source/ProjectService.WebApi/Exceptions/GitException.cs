namespace ProjectService.WebApi.Exceptions;

public class GitException : ApplicationException
{
    public GitException(string message) : base(message)
    {
    }
}