using WebApi.Shared.Results;

namespace WebApi.Features.Identity.Domain;

public static class IdentityErrors
{
    public static Error ProviderNotSupported(string provider) =>
        Error.Validation(
            "auth.provider_not_supported",
            $"El provider '{provider}' no está soportado."
        );

    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "auth.invalid_credentials",
        "Credenciales inválidas."
    );

    public static readonly Error UserNotFound = Error.Unauthorized(
        "auth.user_not_found",
        "No se encontró el usuario."
    );

    public static readonly Error UserLocked = Error.Forbidden(
        "auth.user_locked",
        "El usuario está bloqueado."
    );

    public static readonly Error ExternalLoginInfoNotFound = Error.Failure(
        "auth.external_info_not_found",
        "No se pudo obtener la información del proveedor externo."
    );

    public static readonly Error ExternalEmailNotFound = Error.Validation(
        "auth.external_email_not_found",
        "El proveedor externo no devolvió un email válido."
    );

    public static readonly Error RefreshTokenNotFound = Error.Unauthorized(
        "auth.refresh_token_not_found",
        "No se encontró el refresh token."
    );

    public static Error EmailAlreadyExists(string email) =>
        Error.Conflict("user.email_already_exists", $"Ya existe un usuario con email '{email}'.");
}
