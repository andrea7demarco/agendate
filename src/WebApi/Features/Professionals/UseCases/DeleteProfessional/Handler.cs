using WebApi.Features.Professionals.UseCases.GetProfessionalById; // Importar el otro handler
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.DeleteProfessional;

public class DeleteProfessionalHandler
{
    private readonly IApplicationDbContext _context;
    private readonly GetProfessionalByIdHandler _getByIdHandler;

    public DeleteProfessionalHandler(
        IApplicationDbContext context,
        GetProfessionalByIdHandler getByIdHandler
    )
    {
        _context = context;
        _getByIdHandler = getByIdHandler;
    }

    public async Task<Result> HandleAsync(int id, CancellationToken ct)
    {
        // Usar el handler de GetProfessionalById
        var getResult = await _getByIdHandler.HandleAsync(id, ct);
        if (getResult.IsFailure)
            return Result.Failure(getResult.Error!);

        var professional = await _context.Professionals.FindAsync(id, ct);
        if (professional is not null)
        {
            _context.Professionals.Remove(professional);
            await _context.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
