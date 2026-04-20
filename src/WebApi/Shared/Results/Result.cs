namespace WebApi.Shared.Results;

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Un resultado exitoso no puede tener error.");
        if (!isSuccess && error is null)
            throw new InvalidOperationException("Un resultado fallido debe tener error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    //    public static Result Success(List<Features.Professionals.UseCases.ListProfessionals.ProfessionalResponse> professionals) => new(true, null);
    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);
}
