namespace Daunt.Shared;

public class Result<T> where T : class
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public IEnumerable<ValidationPropertyResult> ErrorMessages { get; init; }

    private Result() { }
    
    public static Result<T> Success() =>
        new Result<T>
        {
            IsSuccess = true
        };
    
    public static Result<T> Success(T value) =>
        new Result<T>
        {
            IsSuccess = true,
            Value = value
        };

    public static Result<T> Failure( IEnumerable<ValidationPropertyResult> errorMessages) =>
        new Result<T>
        {
            IsSuccess = false,
            ErrorMessages = errorMessages
        };
}
