using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FULLSTACKFURY.EduSpace.API.IoTMonitoring.Interfaces.REST.Filters;

/// <summary>
/// Validates the shared-secret header (<c>X-Edge-Key</c>) the Edge API sends when its
/// <c>EDUSPACE_FORWARD_AUTH</c> is configured (see backend-integration-guide §8).
/// <para>
/// Opt-in by design: if no <c>Edge:ApiKey</c> is configured the check is skipped and the
/// endpoint stays open. This prevents the edge's retry-forever loop on <c>401</c> — never
/// turn on enforcement before the edge has the matching secret set.
/// </para>
/// </summary>
public class EdgeApiKeyFilter(IConfiguration configuration) : IActionFilter
{
    private const string HeaderName = "X-Edge-Key";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var expected = configuration["Edge:ApiKey"];
        if (string.IsNullOrEmpty(expected))
            return; // not configured → auth disabled, endpoint open

        var provided = context.HttpContext.Request.Headers[HeaderName].FirstOrDefault();
        if (provided != expected)
            context.Result = new UnauthorizedResult();
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
