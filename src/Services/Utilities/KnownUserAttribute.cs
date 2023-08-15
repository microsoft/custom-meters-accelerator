using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Security.Claims;

namespace ManagedApplicationScheduler.Services.Utilities;

/// <summary>
/// Authorize attribute to check if the user is a known user.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute" />
/// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter" />
public class KnownUserAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    /// <summary>
    /// The known users repository.
    /// </summary>

    private readonly KnownUsersModel knownUsersList;

    /// <summary>
    /// Initializes a new instance of the <see cref="KnownUserAttribute" /> class.
    /// </summary>
    /// <param name="knownUsersRepository">The known users repository.</param>
    /// <param name="knownUsers">The known users.</param>

    public KnownUserAttribute(KnownUsersModel knownUsers)
    {

        this.knownUsersList = knownUsers;
    }


    /// <summary>
    /// Called early in the filter pipeline to confirm request is authorized.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isKnownuser = false;
        string email = string.Empty;

        if (context.HttpContext != null && context.HttpContext.User.Claims.Any())
        {
            email = context.HttpContext.User?.Claims?.Where(s => s.Type == ClaimConstants.CLAIM_EMAILADDRESS)?.FirstOrDefault()?.Value;

            if(email == null)
            {
                var claimlist = context.HttpContext.User?.Claims?.Where(s => s.Type == ClaimConstants.CLAIM_NAME)?.ToList();
                foreach(var claim in claimlist)
                {
                    if (claim.Value.Contains("@", System.StringComparison.Ordinal))
                    {
                        email = claim.Value;
                    }
                }
            }
            //Attempt again with Name. In some cases the claim will have the email under name
            



            if (email != null)
            {
                isKnownuser = knownUsersList.KnownUsers.Contains(email);
            }

            if (!isKnownuser)
            {
                var routeValues = new RouteValueDictionary
                {
                    ["controller"] = "Account",
                    ["action"] = "AccessDenied"
                };
                context.Result = new RedirectToRouteResult(routeValues);
            }
        }
        else
        {
            var routeValues = new RouteValueDictionary
            {
                ["controller"] = "Account",
                ["action"] = "SignIn"
            };
            context.Result = new RedirectToRouteResult(routeValues);
        }
    }
}