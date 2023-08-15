using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace ManagedApplicationScheduler.AdminSite.Controllers
{

    /// <summary>
    /// Defines the <see cref="AccountController" />.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class AccountController : Controller
    {
        /// <summary>
        /// The SignIn.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult" />.
        /// </returns>
        [HttpGet]
        public IActionResult SignIn()
        {
            return this.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// The SignOut.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult" />.
        /// </returns>
        [HttpGet]
        public new SignOutResult SignOut()
        {
            return this.SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = "Home/Index/",
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// The SignedOut.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult" />.
        /// </returns>
        [HttpGet]
        public IActionResult SignedOut() => this.View();
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return this.View("Error");
        }
    }
}