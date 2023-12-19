namespace AuthenticationService.Core.Exceptions;

public class UsernamePasswordException : Exception
{
    public UsernamePasswordException() : base() { }
    public UsernamePasswordException(string msg) : base(msg) { }
}
