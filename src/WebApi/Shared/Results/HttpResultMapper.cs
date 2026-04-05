using Microsoft.AspNetCore.Mvc;

namespace WebApi.Shared.Results;

public static class HttpResultMapper
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
            return TypedResults.NoContent();

        return ToProblem(result.Error!);
    }

    public static IResult ToHttpResult<T>(
        this Result<T> result,
        int successStatusCode = StatusCodes.Status200OK
    )
    {
        if (result.IsFailure)
            return ToProblem(result.Error!);

        return successStatusCode switch
        {
            StatusCodes.Status200OK => TypedResults.Ok(result.Value),
            StatusCodes.Status201Created => TypedResults.StatusCode(StatusCodes.Status201Created),
            _ => TypedResults.Ok(result.Value),
        };
    }

    private static IResult ToProblem(Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        if (error.Type == ErrorType.Validation && error.Metadata is not null)
        {
            return TypedResults.ValidationProblem(
                error.Metadata.ToDictionary(x => x.Key, x => x.Value),
                title: error.Message
            );
        }

        return TypedResults.Problem(
            new ProblemDetails
            {
                Title = error.Message,
                Detail = error.Message,
                Status = status,
                Type = $"https://httpstatuses.com/{status}",
                Extensions = { ["code"] = error.Code },
            }
        );
    }
}
