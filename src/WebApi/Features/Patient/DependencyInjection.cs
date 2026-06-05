using FluentValidation;
using WebApi.Features.Patients.UseCases.CreatePatient;

namespace WebApi.Features.Patients;

public static class DependencyInjection
{
    public static IServiceCollection AddPatientFeatures(this IServiceCollection services)
    {
        services.AddScoped<CreatePatientHandler>();
        services.AddValidatorsFromAssemblyContaining<CreatePatientRequestValidator>();

        return services;
    }

    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/patients").WithTags("Patients");

        UseCases.CreatePatient.Endpoint.Map(group);

        return app;
    }
}
