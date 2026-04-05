namespace WebApi.Shared.Results;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        if (result.IsFailure)
            return Result<TOut>.Failure(result.Error!);

        return Result<TOut>.Success(mapper(result.Value));
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> binder
    )
    {
        var result = await resultTask;

        if (result.IsFailure)
            return Result<TOut>.Failure(result.Error!);

        return await binder(result.Value);
    }

    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder
    )
    {
        if (result.IsFailure)
            return Result<TOut>.Failure(result.Error!);

        return binder(result.Value);
    }
}
