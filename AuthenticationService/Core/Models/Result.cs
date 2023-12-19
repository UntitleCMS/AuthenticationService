namespace AuthenticationService.Core.Models;

public class Result<T>
{
    public virtual T? Value { get; set; }
    public virtual Exception? Errors { get; set; }
    public bool IsSuccess => Errors is null;
}
