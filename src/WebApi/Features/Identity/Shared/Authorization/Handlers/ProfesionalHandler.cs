using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApi.Features.Identity.Domain;

namespace WebApi.Features.Identity.Shared.Authorization.Handlers;

public class ProfesionalHandler(UserManager<ApplicationUser> userManager)
    : AuthorizationHandler<ProfesionalRequirement>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProfesionalRequirement requirement
    )
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
            return;

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return;

        if (
            await _userManager.IsInRoleAsync(user, IdentityRoles.PROFESIONAL)
            && user.RegistrationCompleted
        )
            context.Succeed(requirement);

        context.Fail();
    }
}

public class ProfesionalRequirement : IAuthorizationRequirement { }
