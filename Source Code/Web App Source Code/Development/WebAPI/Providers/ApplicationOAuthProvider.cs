using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using IID.WebAPI.Models;
using IID.BusinessLayer.Identity;

namespace IID.WebAPI.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            ApplicationSignInManager signInManager = context.OwinContext.Get<ApplicationSignInManager>();

            SignInStatus result = await signInManager.PasswordSignInAsync(context.UserName, context.Password, false, true);
            IidUser user = await userManager.FindByEmailAsync(context.UserName);
            switch (result)
            {
                case SignInStatus.Success:

                    //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                    //   OAuthDefaults.AuthenticationType);
                    //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                    //    CookieAuthenticationDefaults.AuthenticationType);
                    ClaimsIdentity identity = await user.GenerateUserIdentityAsync(userManager);

                    AuthenticationProperties properties = CreateProperties(user.UserName);
                    AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
                    context.Validated(ticket);
                    context.Request.Context.Authentication.SignIn(identity);
                    break;

                case SignInStatus.Failure:
                    context.SetError("invalid_attempt", "The user name or password is incorrect.");
                    break;

                case SignInStatus.LockedOut:
                    context.SetError("locked_out", user.LastTimeBox.Value.ToUniversalTime().ToString());
                  break;

                case SignInStatus.RequiresVerification:
                    context.SetError("reset_password", "Reset password email has been sent.");
                    break;
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}