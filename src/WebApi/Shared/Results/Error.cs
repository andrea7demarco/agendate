namespace WebApi.Shared.Results;

public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    IReadOnlyDictionary<string, string[]>? Metadata = null
)
{
    public static Error Validation(
        string code,
        string message,
        IReadOnlyDictionary<string, string[]>? metadata = null
    ) => new(code, message, ErrorType.Validation, metadata);

    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    internal static Error BadRequest(string v)
    {
        throw new NotImplementedException();
    }
}
