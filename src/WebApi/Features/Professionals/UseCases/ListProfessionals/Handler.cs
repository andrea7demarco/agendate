using Microsoft.EntityFrameworkCore;
using WebApi.Features.Professionals.Domain;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.ListProfessionals;

public class ListProfessionalsHandler
{
    private readonly IApplicationDbContext _context;

    public ListProfessionalsHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<PaginatedProfessionalsResponse>> HandleAsync(
        ListProfessionalsRequest request,
        CancellationToken ct
    )
    {
        var requestedPage = Math.Max(request.Page ?? 1, 1);
        var pageSize = Math.Clamp(request.PageSize ?? 8, 1, 50);
        var query = _context.Professionals.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search.Trim()}%";
            var matchingSpecialtyIds = await _context
                .Specialties.AsNoTracking()
                .Where(s => EF.Functions.ILike(s.Name, searchPattern))
                .Select(s => s.Id)
                .ToListAsync(ct);

            var searchableSpecialtyIds = new List<int>();
            if (matchingSpecialtyIds.Count > 0)
            {
                var specialtyTree = await LoadSpecialtyTreeAsync(ct);
                searchableSpecialtyIds = matchingSpecialtyIds
                    .SelectMany(id => GetDescendantIds(specialtyTree, id))
                    .Distinct()
                    .ToList();
            }

            query = query.Where(p =>
                EF.Functions.ILike(p.FirstName + " " + p.LastName, searchPattern)
                || EF.Functions.ILike(p.Province ?? string.Empty, searchPattern)
                || EF.Functions.ILike(p.Address ?? string.Empty, searchPattern)
                || EF.Functions.ILike(p.Biography ?? string.Empty, searchPattern)
                || p.ProfessionalSpecialties.Any(ps =>
                    EF.Functions.ILike(ps.Specialty.Name, searchPattern)
                )
                || p.ProfessionalSpecialties.Any(ps =>
                    searchableSpecialtyIds.Contains(ps.SpecialtyId)
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Province))
        {
            var province = request.Province.Trim();
            query = query.Where(p =>
                p.Province != null && EF.Functions.ILike(p.Province, province)
            );
        }

        if (
            !string.IsNullOrWhiteSpace(request.AppointmentType)
            && Enum.TryParse<AppointmentType>(
                request.AppointmentType,
                ignoreCase: true,
                out var appointmentType
            )
        )
        {
            query =
                appointmentType == AppointmentType.Ambos
                    ? query.Where(p => p.AppointmentType == AppointmentType.Ambos)
                    : query.Where(p =>
                        p.AppointmentType == appointmentType
                        || p.AppointmentType == AppointmentType.Ambos
                    );
        }

        if (request.SpecialtyId.HasValue)
        {
            var specialties = await LoadSpecialtyTreeAsync(ct);
            var includedIds = GetDescendantIds(specialties, request.SpecialtyId.Value);
            query = query.Where(p =>
                p.ProfessionalSpecialties.Any(ps => includedIds.Contains(ps.SpecialtyId))
            );
        }

        var totalCount = await query.CountAsync(ct);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var page = totalPages == 0 ? 1 : Math.Min(requestedPage, totalPages);

        var professionals = await query
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProfessionalResponse(
                p.Id,
                p.ApplicationUserId,
                p.FirstName + " " + p.LastName,
                p.Email,
                p.PhoneNumber,
                p.ConsultationCost,
                p.AppointmentType.ToString(),
                p.Address,
                p.Province,
                p.NationalLicense,
                p.ProvincialLicense,
                p.Biography,
                p.ProfessionalSpecialties.Select(ps => ps.Specialty.Name).ToList()
            ))
            .ToListAsync(ct);

        var response = new PaginatedProfessionalsResponse(
            professionals,
            page,
            pageSize,
            totalCount,
            totalPages
        );

        return Result<PaginatedProfessionalsResponse>.Success(response);
    }

    private Task<List<SpecialtyNode>> LoadSpecialtyTreeAsync(CancellationToken ct) =>
        _context
            .Specialties.AsNoTracking()
            .Select(s => new SpecialtyNode(s.Id, s.ParentSpecialtyId))
            .ToListAsync(ct);

    private static List<int> GetDescendantIds(IEnumerable<SpecialtyNode> specialties, int rootId)
    {
        var relations = specialties.ToList();
        var result = new List<int> { rootId };

        for (var index = 0; index < result.Count; index++)
        {
            var parentId = result[index];
            result.AddRange(
                relations
                    .Where(s => s.ParentSpecialtyId == parentId && !result.Contains(s.Id))
                    .Select(s => s.Id)
            );
        }

        return result;
    }

    private sealed record SpecialtyNode(int Id, int? ParentSpecialtyId);
}
