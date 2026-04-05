using Microsoft.AspNetCore.Identity;
using WebApi.Features.Identity.Shared.Authorization;

namespace WebApi.Features.Identity.Shared.Persistence;

public sealed class IdentityRoleSeeder(RoleManager<IdentityRole> roleManager)
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        foreach (var roleName in IdentityRoles.ALL)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new InvalidOperationException(
                    $"No se pudo crear el rol '{roleName}'. Errores: {errors}"
                );
            }
        }
    }
}
