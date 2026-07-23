using WebApi.Features.HealthInsurances.UseCases.ListHealthInsurances;

namespace WebApi.Features.HealthInsurances;

public static class DependencyInjection
{
    public static IServiceCollection AddHealthInsuranceFeatures(this IServiceCollection services)
    {
        services.AddScoped<ListHealthInsurancesHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapHealthInsuranceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/health-insurances").WithTags("Health Insurances");

        UseCases.ListHealthInsurances.Endpoint.Map(group);

        return app;
    }
}
