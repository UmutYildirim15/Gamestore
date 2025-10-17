using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Utilities;

public class HasAnyPermissionAttribute(params string[] permissions) : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool hasClaim = permissions.Any(p =>
            context.HttpContext.User.HasClaim(c => c.Type == "permission" && c.Value == p));

        if (!hasClaim)
        {
            context.Result = new ForbidResult();
        }
    }
}