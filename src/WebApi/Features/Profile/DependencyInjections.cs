using WebApi.Features.Profiles.UseCases.GetMyProfile;

namespace WebApi.Features.Profiles;

public static class DependencyInjection
{
    public static IServiceCollection AddProfilesFeature(this IServiceCollection services)
    {
        services.AddScoped<GetMyProfileHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapProfilesModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profile").WithTags("Profile");

        WebApi.Features.Profiles.UseCases.GetMyProfile.Endpoint.Map(group);

        return app;
    }
}
