using Microsoft.Extensions.DependencyInjection;
using WebApi.Features.Professionals.UseCases.ListProfessionals; // ← importa el namespace

namespace WebApi.Features.Professionals;

public static class DependencyInjection
{
    public static IServiceCollection AddProfessionalFeatures(this IServiceCollection services)
    {
        services.AddScoped<ListProfessionalsHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapProfessionalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/professionals").WithTags("Professionals");
        UseCases.ListProfessionals.Endpoint.Map(group);
        return app;
    }
}
