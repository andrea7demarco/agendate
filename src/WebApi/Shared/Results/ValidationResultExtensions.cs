using FluentValidation.Results;

namespace WebApi.Shared.Results;

public static class FluentValidationResultExtensions
{
    public static Result ToResult(
        this ValidationResult validationResult,
        string errorCode = "validation.error"
    )
    {
        if (validationResult.IsValid)
            return Result.Success();

        var errors = validationResult
            .Errors.GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).Distinct().ToArray());

        return Result.Failure(
            Error.Validation(errorCode, "Se produjeron uno o más errores de validación.", errors)
        );
    }
}
