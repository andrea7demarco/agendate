using WebApi.Features.Patients.UseCases.GetPatientById;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.DeletePatient;

public class DeletePatientHandler
{
    private readonly IApplicationDbContext _context;
    private readonly GetPatientByIdHandler _getByIdHandler;

    public DeletePatientHandler(IApplicationDbContext context, GetPatientByIdHandler getByIdHandler)
    {
        _context = context;
        _getByIdHandler = getByIdHandler;
    }

    public async Task<Result> HandleAsync(int id, CancellationToken ct)
    {
        // Usar el handler de GetPatientById
        var getResult = await _getByIdHandler.HandleAsync(id, ct);
        if (getResult.IsFailure)
            return Result.Failure(getResult.Error!);

        var patient = await _context.Patients.FindAsync(id, ct);
        if (patient is not null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
