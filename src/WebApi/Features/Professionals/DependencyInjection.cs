using Microsoft.Extensions.DependencyInjection;
using WebApi.Features.Professionals.UseCases.CreateProfessional;
using WebApi.Features.Professionals.UseCases.DeleteProfessional;
using WebApi.Features.Professionals.UseCases.GetProfessionalById;
using WebApi.Features.Professionals.UseCases.ListProfessionals;
using WebApi.Features.Professionals.UseCases.ListSpecialties;
using WebApi.Features.Professionals.UseCases.UpdateProfessional;

namespace WebApi.Features.Professionals;

public static class DependencyInjection
{
    public static IServiceCollection AddProfessionalFeatures(this IServiceCollection services)
    {
        services.AddScoped<ListProfessionalsHandler>();
        services.AddScoped<ListSpecialtiesHandler>();
        services.AddScoped<CreateProfessionalHandler>();
        services.AddScoped<GetProfessionalByIdHandler>();
        services.AddScoped<DeleteProfessionalHandler>();
        services.AddScoped<UpdateProfessionalHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapProfessionalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/professionals").WithTags("Professionals");
        UseCases.ListProfessionals.Endpoint.Map(group);
        UseCases.ListSpecialties.Endpoint.Map(group);
        UseCases.CreateProfessional.Endpoint.Map(group);
        UseCases.GetProfessionalById.Endpoint.Map(group);
        UseCases.DeleteProfessional.Endpoint.Map(group);
        UseCases.UpdateProfessional.Endpoint.Map(group);
        return app;
    }

}
