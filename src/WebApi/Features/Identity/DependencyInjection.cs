using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Identity.Shared.Auth;
using WebApi.Features.Identity.Shared.Authorization;
using WebApi.Features.Identity.Shared.Persistence;
using WebApi.Features.Identity.UseCases.CompleteRegistration;
using WebApi.Features.Identity.UseCases.Login.Google;
using WebApi.Features.Identity.UseCases.Login.Local;
using WebApi.Features.Identity.UseCases.Register;
using WebApi.Features.Patients.UseCases.CreatePatient;
using WebApi.Shared.Persistence;

namespace WebApi.Features.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityFeature(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        var jwtOptions =
            configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration missing.");

        services.Configure<GoogleAuthOptions>(
            configuration.GetSection(GoogleAuthOptions.SectionName)
        );

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwtOptions.Key)
                    ),
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization(Policies.RegisterPolicies);

        services.AddScoped<ITokenProvider, TokenProvider>();

        services.AddScoped<LocalLoginHandler>();
        services.AddScoped<GoogleLoginHandler>();
        services.AddScoped<CompleteRegistrationHandler>();
        services.AddScoped<CreatePatientHandler>();
        services.AddScoped<RegisterHandler>();
        services.AddScoped<IdentityRoleSeeder>();

        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<LocalLoginRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<GoogleLoginRequest>();
        services.AddValidatorsFromAssemblyContaining<CompleteRegistrationRequestValidator>();

        return services;
    }

    public static IEndpointRouteBuilder MapIdentityModule(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity").WithTags("Identity");

        UseCases.Register.Endpoint.Map(group);
        UseCases.CompleteRegistration.Endpoint.Map(group);
        UseCases.Login.Local.Endpoint.Map(group);
        UseCases.Login.Google.Endpoint.Map(group);
        UseCases.Logout.Endpoint.Map(group);
        UseCases.Me.Endpoint.Map(group);

        return app;
    }

    public static async Task SeedIdentityAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IdentityRoleSeeder>();
        await seeder.SeedAsync();
    }
}
