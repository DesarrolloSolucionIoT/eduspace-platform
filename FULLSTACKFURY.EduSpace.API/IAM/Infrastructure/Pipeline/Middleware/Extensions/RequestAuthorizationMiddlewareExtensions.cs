using FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Pipeline.Middleware.Components;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Pipeline.Middleware.Extensions;

public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}