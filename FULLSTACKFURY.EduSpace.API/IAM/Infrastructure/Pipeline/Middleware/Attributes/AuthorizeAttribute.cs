using FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FULLSTACKFURY.EduSpace.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    ///     On Authorization
    /// </summary>
    /// <remarks>
    ///     This method is called when the authorization filter is executed.
    ///     It checks if the user is authenticated and if not, it returns an unauthorized result.
    /// </remarks>
    /// <param name="context">
    ///     The <see cref="AuthorizationFilterContext" /> Authorization Filter Context
    /// </param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
        {
            Console.WriteLine("Allow Anonymous");
            return;
        }

        var account = (Account?)context.HttpContext.Items["Account"];
        if (account is null) context.Result = new UnauthorizedResult();
    }
}