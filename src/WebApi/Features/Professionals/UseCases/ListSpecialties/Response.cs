namespace WebApi.Features.Professionals.UseCases.ListSpecialties;

public record SpecialtyResponse(int Id, string Name, int? ParentSpecialtyId);
