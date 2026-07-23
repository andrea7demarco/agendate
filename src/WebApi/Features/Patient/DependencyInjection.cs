using FluentValidation;
using WebApi.Features.Patients.UseCases.CreatePatient;
using WebApi.Features.Patients.UseCases.DeletePatient;
using WebApi.Features.Patients.UseCases.GetPatientById;
using WebApi.Features.Patients.UseCases.ListPatients;
using WebApi.Features.Patients.UseCases.UpdatePatient;

namespace WebApi.Features.Patients;

public static class DependencyInjection
{
    public static IServiceCollection AddPatientFeatures(this IServiceCollection services)
    {
        services.AddScoped<CreatePatientHandler>();
        services.AddValidatorsFromAssemblyContaining<CreatePatientRequestValidator>();
        services.AddScoped<ListPatientsHandler>();
        services.AddScoped<GetPatientByIdHandler>();
        services.AddScoped<DeletePatientHandler>();
        services.AddScoped<UpdatePatientHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients").WithTags("Patients");

        UseCases.CreatePatient.Endpoint.Map(group);
        UseCases.ListPatients.Endpoint.Map(group);
        UseCases.GetPatientById.Endpoint.Map(group);
        UseCases.DeletePatient.Endpoint.Map(group);
        UseCases.UpdatePatient.Endpoint.Map(group);
        return app;
    }
}
