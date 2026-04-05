using Microsoft.AspNetCore.Identity;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Identity.Shared.Authorization;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Register;

public sealed class RegisterHandler(UserManager<ApplicationUser> userManager)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<RegisterResponse>> HandleAsync(
        RegisterRequest request,
        CancellationToken ct
    )
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Result<RegisterResponse>.Failure(
                IdentityErrors.EmailAlreadyExists(request.Email)
            );

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true,
            RegistrationCompleted = request.RegistrationKind != IdentityRoles.PROFESIONAL,
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return Result<RegisterResponse>.Failure(
                Error.Validation(
                    "auth.user_creation_failed",
                    "No se pudo crear el usuario.",
                    createResult
                        .Errors.GroupBy(x => x.Code)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.Description).ToArray())
                )
            );
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, request.RegistrationKind);
        if (!addRoleResult.Succeeded)
        {
            return Result<RegisterResponse>.Failure(
                Error.Validation(
                    "auth.user_role_assignment_failed",
                    "No se pudo asignar el rol base al usuario.",
                    addRoleResult
                        .Errors.GroupBy(x => x.Code)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.Description).ToArray())
                )
            );
        }

        return Result<RegisterResponse>.Success(
            new RegisterResponse(
                UserId: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName
            )
        );
    }
}
