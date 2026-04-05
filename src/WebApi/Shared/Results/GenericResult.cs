namespace WebApi.Shared.Results;

public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, null)
    {
        _value = value;
    }

    private Result(Error error)
        : base(false, error)
    {
        _value = default;
    }

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "No se puede acceder a Value en un resultado fallido."
            );

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => new(error);
}
