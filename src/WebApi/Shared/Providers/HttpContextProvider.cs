namespace WebApi.Shared.Providers;

public interface IHttpContextProvider
{
    HttpContext? GetCurrent();
}

public class HttpContextProvider(IHttpContextAccessor httpContextAccessor) : IHttpContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public HttpContext? GetCurrent() => _httpContextAccessor.HttpContext;
}
