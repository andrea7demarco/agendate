namespace WebApi.Features.Identity.Shared.Authorization;

public static class IdentityRoles
{
    public const string PROFESIONAL = "profesional";
    public const string PACIENTE = "paciente";
    public const string ADMINISTRADOR = "administrador";

    public static string[] ALL = [PROFESIONAL, PACIENTE, ADMINISTRADOR];
}
