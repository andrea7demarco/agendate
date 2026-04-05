using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WebApi.Features.Identity.Shared.Authorization.Handlers;

namespace WebApi.Features.Identity.Shared.Authorization;

public static class Policies
{
    public const string ProfesionalPolicy = "Profesional";

    public static void RegisterPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(
            ProfesionalPolicy,
            policy => policy.AddRequirements(new ProfesionalRequirement())
        );

        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .Build();

        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .Build();
    }
}
