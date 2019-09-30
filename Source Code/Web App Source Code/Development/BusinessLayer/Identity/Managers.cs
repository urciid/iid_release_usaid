using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using System.Security.Principal;

using IID.BusinessLayer.Helpers;
using IID.BusinessLayer.Globalization.Email;

namespace IID.BusinessLayer.Identity
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await BusinessLayer.Helpers.SendGrid.SendEmail(message);
        }
    }

    public class ApplicationUserManager : UserManager<IidUser, int>
    {
        private readonly IUserLockoutStore<IidUser, int> _userLockoutStore;

        public ApplicationUserManager(IUserStore<IidUser, int> store)
            : base(store)
        {
        }

        public ApplicationUserManager(IUserLockoutStore<IidUser, int> store)
            : base(store)
        {
            if (store == null)
                throw new ArgumentNullException("store");

            _userLockoutStore = store;
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new IidUserStore());

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<IidUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = false,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(30);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            manager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<IidUser, int>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public override async Task<IdentityResult> AccessFailedAsync(int userId)
        {
            IidUser user = await FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException(("UserId not found: " + userId.ToString()));

            int accessFailedCount = await _userLockoutStore.IncrementAccessFailedCountAsync(user);

            if (accessFailedCount == Settings.MaxFailedAccessAttemptsBeforePasswordReset)
            {
                // Send the user an email with a link to reset the password.
                await SendForgotPasswordLinkEmail(user);
            }
            else if (accessFailedCount >= MaxFailedAccessAttemptsBeforeLockout)
            {
                // Lock the user out.
                await _userLockoutStore.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.Add(DefaultAccountLockoutTimeSpan));
            }

            return await UpdateAsync(user);
        }

        public override async Task<IdentityResult> ResetPasswordAsync(int userId, string token, string newPassword)
        {
            IidUser user = await FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException(("UserId not found: " + userId.ToString()));

            await _userLockoutStore.SetLockoutEnabledAsync(user, false);
            await _userLockoutStore.SetLockoutEndDateAsync(user, DateTimeOffset.MinValue);
            await _userLockoutStore.ResetAccessFailedCountAsync(user);

            var result = await base.ResetPasswordAsync(userId, token, newPassword);
            if (result.Succeeded)
                await SendResetPasswordConfirmationEmail(user);

            return result;
        }

        public string GetRandomPassword()
        {
            const int passwordLength = 8;
            const int upperCaseCount = 1;
            const int nonAlphaNumericCount = 1;
            int lowerCaseCount = passwordLength - upperCaseCount - nonAlphaNumericCount;
            const string allowedAlpha = "abcdefghijklmnopqrstuvwxyz";
            const string allowedAlphaNumeric = (allowedAlpha + "0123456789");
            const string allowedNonAlphaNumeric = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            char[] randomChars = new char[passwordLength];

            int offset = 0;

            byte[] upperCaseBytes = new byte[upperCaseCount];
            rng.GetBytes(upperCaseBytes);
            for (int i = 0; i < upperCaseCount; i++)
                randomChars[offset + i] = Char.ToUpper(allowedAlpha[upperCaseBytes[i] % allowedAlpha.Length]);
            offset += upperCaseCount;

            byte[] lowerCaseBytes = new byte[lowerCaseCount];
            rng.GetBytes(lowerCaseBytes);
            for (int i = 0; i < lowerCaseCount; i++)
                randomChars[offset + i] = allowedAlphaNumeric[lowerCaseBytes[i] % allowedAlphaNumeric.Length];
            offset += lowerCaseCount;

            byte[] nonAlphaNumericBytes = new byte[nonAlphaNumericCount];
            rng.GetBytes(nonAlphaNumericBytes);
            for (int i = 0; i < nonAlphaNumericCount; i++)
                randomChars[offset + i] = allowedNonAlphaNumeric[nonAlphaNumericBytes[i] % allowedNonAlphaNumeric.Length];

            return new string(randomChars);
        }

        public void InviteUser(string emailAddress)
        {
            var task = FindByEmailAsync(emailAddress);
            InviteUser(task.Result);
        }

        // NOTE: Refactored as synchronous because multiple async calls on the same user was causing problems.
        public void InviteUser(IidUser user)
        {
            var userId = user.Id;

            var resetCodeTask = GeneratePasswordResetTokenAsync(userId);
            string resetCode = resetCodeTask.Result;

            string password = GetRandomPassword();
            var setPasswordTask = base.ResetPasswordAsync(userId, resetCode, password);

            HttpRequest request = HttpContext.Current.Request;
            UrlHelper urlHelper = new UrlHelper(request.RequestContext);
            string callbackUrl = urlHelper.Action("ResetPassword", "Account", new { userId = userId, code = resetCode }, request.Url.Scheme);
            SendEmailAsync(
                user.Id, Resource.WelcomeSubject,
                String.Format(Resource.WelcomeBody, user.FirstName, user.LastName, callbackUrl, Settings.AdminEmailAddress, Settings.UrcChsPhoneNumber));
        }

        public async Task SendForgotPasswordLinkEmail(string emailAddress)
        {
            await SendForgotPasswordLinkEmail(await FindByNameAsync(emailAddress));
        }

        public async Task SendForgotPasswordLinkEmail(IidUser user)
        {
            // NOTE: Don't send an error if the user is null.
            // Example -- Someone is trying to guess random email addresses.
            if (user != null)
            {
                string code = await GeneratePasswordResetTokenAsync(user.Id);
                HttpRequest request = HttpContext.Current.Request;
                UrlHelper urlHelper = new UrlHelper(request.RequestContext);
                string callbackUrl = urlHelper.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, request.Url.Scheme);
                await SendEmailAsync(
                    user.Id, Resource.ForgotPasswordSubject,
                    String.Format(Resource.ForgotPasswordBody, user.FirstName, user.LastName, callbackUrl, Settings.AdminEmailAddress, Settings.UrcChsPhoneNumber));
            }
        }

        public async Task SendResetPasswordConfirmationEmail(IidUser user)
        {
            await SendEmailAsync(
                user.Id, Resource.PasswordChangedSubject,
                String.Format(Resource.PasswordChangedBody, user.FirstName, user.LastName, Settings.AdminEmailAddress, Settings.UrcChsPhoneNumber));
        }
    }

    public class ApplicationSignInManager : SignInManager<IidUser, int>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        public static AuthenticationContext AuthContext =
            new AuthenticationContext(
                String.Format(CultureInfo.InvariantCulture,
                              Settings.ActiveDirectory.LoginUrl,
                              Settings.ActiveDirectory.Tenant));

        public override async Task<ClaimsIdentity> CreateUserIdentityAsync(IidUser user)
        {
            //return CreateUserIdentityAsync(user);
            return await user.GenerateUserIdentityAsync(UserManager);
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            SignInStatus result;
            if (UserManager == null)
            {
                result = SignInStatus.Failure;
            }
            else
            {
                IidUser user = await UserManager.FindByNameAsync(userName);
                if (user == null || !(user.Active ?? false))
                {
                    // User does not exist, or is not active.
                    result = SignInStatus.Failure;
                }
                else
                {
                    // Get the access failed count BEFORE this login attempt.
                    int accessFailedCount = await UserManager.GetAccessFailedCountAsync(user.Id);

                    if (accessFailedCount == Settings.MaxFailedAccessAttemptsBeforePasswordReset)
                    {
                        // User cannot login until password is reset.
                        result = SignInStatus.RequiresVerification;
                    }
                    else if (await UserManager.IsLockedOutAsync(user.Id))
                    {
                        // User is locked out.
                        result = SignInStatus.LockedOut;
                    }
                    else
                    {
                        // Authenticate the user.
                        bool passwordIsValid = false;
                        if (userName.ToLower().EndsWith(Settings.ActiveDirectory.Domain.ToLower()))
                        {
                            // Authenticate via Azure Active Directory.
                            AuthenticationResult r = null;
                            try
                            {
                                r = await AuthContext.AcquireTokenAsync(
                                        Settings.ActiveDirectory.ResourceId,
                                        Settings.ActiveDirectory.ClientId,
                                        new UserPasswordCredential(userName, password));
                            }
                            catch(Exception)
                            {
                                // Ignore errors for now.
                            }
                            passwordIsValid = (r != null && r.AccessToken != null);
                        }
                        else
                        {
                            // Authenticate via database.
                            passwordIsValid = await UserManager.CheckPasswordAsync(user, password);
                        }

                        if (passwordIsValid)
                        {
                            await UserManager.ResetAccessFailedCountAsync(user.Id);
                            result = await SignInOrTwoFactor(user, isPersistent);
                        }
                        else
                        {
                            if (shouldLockout)
                            {
                                await UserManager.AccessFailedAsync(user.Id);
                                accessFailedCount = await UserManager.GetAccessFailedCountAsync(user.Id);

                                if (accessFailedCount == Settings.MaxFailedAccessAttemptsBeforePasswordReset)
                                {
                                    // The system has emailed a "reset password" link to the user.
                                    return SignInStatus.RequiresVerification;
                                }
                                else if (await UserManager.IsLockedOutAsync(user.Id))
                                {
                                    return SignInStatus.LockedOut;
                                }
                            }
                            result = SignInStatus.Failure;
                        }
                    }
                }
            }
            return result;
        }

        private async Task<SignInStatus> SignInOrTwoFactor(IidUser user, bool isPersistent)
        {
            string text = user.Id.ToString();
            SignInStatus result;

            if (await UserManager.GetTwoFactorEnabledAsync(user.Id) && (await UserManager.GetValidTwoFactorProvidersAsync(user.Id)).Count > 0 && !(await AuthenticationManager.TwoFactorBrowserRememberedAsync(text)))
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity("TwoFactorCookie");
                claimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", text));
                AuthenticationManager.SignIn(new ClaimsIdentity[] { claimsIdentity });
                result = SignInStatus.RequiresVerification;
            }
            else
            {
                await SignInAsync(user, isPersistent, false);
                (UserManager as ApplicationUserManager).SetLastLoginActivity(user);
                result = SignInStatus.Success;
            }

            return result;
        }
    }

    public static class ApplicationUserManagerExtensions
    {
        public static IdentityResult SetLastLoginActivity(this ApplicationUserManager manager, IidUser user)
        {
            user.LastLoginActivity = DateTime.Now;
            return manager.Update(user);
        }
    }
}
