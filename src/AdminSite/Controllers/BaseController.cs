using ManagedApplicationScheduler.Services.Models;
using ManagedApplicationScheduler.Services.Configurations;
using ManagedApplicationScheduler.Services.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ManagedApplicationScheduler.AdminSite.Controllers
{

    /// <summary>
    ///  Sets a BaseController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class BaseController : Controller
    {
    
#pragma warning disable CS0649 // Field 'BaseController.knownUsersModel' is never assigned to, and will always have its default value null
        private readonly KnownUsersModel knownUsersModel;
#pragma warning restore CS0649 // Field 'BaseController.knownUsersModel' is never assigned to, and will always have its default value null
        /// <summary>
        /// Gets Current Logged in User Email Address.
        /// </summary>
        /// <value>
        /// The current user email address.
        /// </value>
        private string CurrentUserEmailAddress
        {
            get
            {
                return HttpContext?.User?.Claims?.FirstOrDefault(s => s.Type == ClaimConstants.CLAIM_EMAILADDRESS)?.Value ?? string.Empty;
            }
        }


        /// <summary>
        /// Checks the authentication.
        /// </summary>
        /// <returns>
        /// Check authentication.
        /// </returns>
        [HttpGet]
        public IActionResult CheckAuthentication()
        {
            
            if (this.HttpContext == null || !this.HttpContext.User.Identity.IsAuthenticated)
            {
                
                return this.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
            }
            else
            { 
                if(this.knownUsersModel.KnownUsers.Contains(this.CurrentUserEmailAddress,System.StringComparison.OrdinalIgnoreCase))
                {
                    return this.RedirectToAction("Index", "Home", new { });
                }
                else
                {
                    return this.RedirectToAction("Error", "Access Denied");
                }
                
            }
        }
    }
}