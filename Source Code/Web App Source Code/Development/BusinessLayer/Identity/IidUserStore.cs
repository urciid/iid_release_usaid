using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace IID.BusinessLayer.Identity
{
    public class IidUserStore :
        IUserStore<IidUser, int>,
        IUserPasswordStore<IidUser, int>,
        IUserSecurityStampStore<IidUser, int>,
        IUserLockoutStore<IidUser, int>,
        IUserEmailStore<IidUser, int>,
        IUserTwoFactorStore<IidUser, int>,
        IUserClaimStore<IidUser, int>
    {
        private UserStore<IdentityUser> _userStore;
        private UserStore<IdentityUser> UserStore
        {
            get { return this._userStore; }
        }

        private IdentityModel Context
        {
            get { return (IdentityModel)this.UserStore.Context; }
        }

        public IidUserStore()
        {
            this._userStore = new UserStore<IdentityUser>(new IdentityModel());
        }

        public Task CreateAsync(IidUser user)
        {
            this.Context.Users.Add(user);
            this.Context.Configuration.ValidateOnSaveEnabled = false;
            return this.Context.SaveChangesAsync();
        }

        public Task DeleteAsync(IidUser user)
        {
            this.Context.Users.Remove(user);
            this.Context.Configuration.ValidateOnSaveEnabled = false;
            return this.Context.SaveChangesAsync();
        }

        public Task<IidUser> FindByIdAsync(int userId)
        {
            return this.Context.Users.Include(e => e.UserRoles).Where(e => e.Id == userId).FirstOrDefaultAsync();
        }

        public Task<IidUser> FindByNameAsync(string userName)
        {
            return this.Context.Users
                .Where(u => u.UserName.ToLower() == userName.ToLower())
                .Include(e => e.UserRoles)
                .FirstOrDefaultAsync();
        }

        public Task UpdateAsync(IidUser user)
        {
            this.Context.Users.Attach(user);
            this.Context.Entry(user).State = EntityState.Modified;
            this.Context.Configuration.ValidateOnSaveEnabled = false;
            return this.Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._userStore.Dispose();
        }

        public Task<string> GetPasswordHashAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.GetPasswordHashAsync(identityUser);
        }

        public Task<bool> HasPasswordAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.HasPasswordAsync(identityUser);
        }

        public Task SetPasswordHashAsync(IidUser user, string passwordHash)
        {
            var identityUser = this.ToIdentityUser(user);
            var task = this.UserStore.SetPasswordHashAsync(identityUser, passwordHash);
            SetApplicationUser(user, identityUser);
            return task;
        }

        public Task<string> GetSecurityStampAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.GetSecurityStampAsync(identityUser);
        }

        public Task SetSecurityStampAsync(IidUser user, string stamp)
        {
            var identityUser = this.ToIdentityUser(user);
            var task = this.UserStore.SetSecurityStampAsync(identityUser, stamp);
            SetApplicationUser(user, identityUser);
            return task;
        }

        private static void SetApplicationUser(IidUser user, IdentityUser identityUser)
        {
            user.Password = identityUser.PasswordHash;
            user.ConsecutiveFailedLoginAttempts = identityUser.AccessFailedCount;
            user.LastTimeBox = identityUser.LockoutEndDateUtc;
            user.SecurityStamp = identityUser.SecurityStamp;
            // I don't think this is necessaryy but I left it commented in case we do need to turn it on.
            //for (int i = 0; i <  identityUser.Roles.Count; i++)
            //{
            //    var role = identityUser.Roles.ElementAt(i);
            //    user.Claims.Add(new IidUserClaim()
            //    {
            //        ClaimType = ClaimTypes.Role,
            //        ClaimValue = role.RoleId,
            //        Id = i,
            //        UserId = user.Id
            //    });
            //}
        }

        private IdentityUser ToIdentityUser(IidUser user)
        {
            IdentityUser iuser = new IdentityUser()
            {
                Id = user.Id.ToString(),
                PasswordHash = user.Password,
                SecurityStamp = user.SecurityStamp,
                Email = user.UserName,
                UserName = String.Format("{0} {1}", user.FirstName, user.LastName),
                AccessFailedCount = user.ConsecutiveFailedLoginAttempts,
                LockoutEnabled = user.LastTimeBox.HasValue,
                LockoutEndDateUtc = user.LastTimeBox
            };
            // I don't think this is necessaryy but I left it commented in case we do need to turn it on.
            //for (int i = 0; i < user.UserRoles.Count; i++)
            //{
            //    var role = user.UserRoles.ElementAt(i);
            //    iuser.Claims.Add(new IdentityUserClaim()
            //    {
            //        ClaimType = ClaimTypes.Role,
            //        ClaimValue = role.user_role_fieldid,
            //        Id = i,
            //        UserId = role.user_id.ToString()
            //    });
            //}
            return iuser;
        }

        public Task<int> GetAccessFailedCountAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.GetAccessFailedCountAsync(identityUser);
        }

        public Task<bool> GetLockoutEnabledAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.GetLockoutEnabledAsync(identityUser);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.GetLockoutEndDateAsync(identityUser);
        }

        public Task<int> IncrementAccessFailedCountAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            var task = this.UserStore.IncrementAccessFailedCountAsync(identityUser);
            SetApplicationUser(user, identityUser);
            return task;
        }

        public Task ResetAccessFailedCountAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            var task = this.UserStore.ResetAccessFailedCountAsync(identityUser);
            SetApplicationUser(user, identityUser);
            return task;
        }

        public Task SetLockoutEnabledAsync(IidUser user, bool enabled)
        {
            var identityUser = this.ToIdentityUser(user);
            var task = this.UserStore.SetLockoutEnabledAsync(identityUser, enabled);
            SetApplicationUser(user, identityUser);
            return task;
        }

        public Task SetLockoutEndDateAsync(IidUser user, DateTimeOffset lockoutEnd)
        {
            var identityUser = this.ToIdentityUser(user);
            var task = this.UserStore.SetLockoutEndDateAsync(identityUser, lockoutEnd);
            SetApplicationUser(user, identityUser);
            return task;
        }

        public Task SetEmailAsync(IidUser user, string email)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.SetEmailAsync(identityUser, email);
        }

        public Task<string> GetEmailAsync(IidUser user)
        {
            var identityUser = this.ToIdentityUser(user);
            return this.UserStore.GetEmailAsync(identityUser);
        }

        public Task<bool> GetEmailConfirmedAsync(IidUser user)
        {
            return Task.Factory.StartNew<bool>(() => true);
        }

        public Task SetEmailConfirmedAsync(IidUser user, bool confirmed)
        {
            return Task.Factory.StartNew<bool>(() => true);
        }

        public Task<IidUser> FindByEmailAsync(string email)
        {
            var context = this.UserStore.Context as IdentityModel;
            return context.Users.Where(u => u.UserName == email).FirstOrDefaultAsync();
        }

        public Task SetTwoFactorEnabledAsync(IidUser user, bool enabled)
        {
            return Task.Factory.StartNew<bool>(() => false);
        }

        public Task<bool> GetTwoFactorEnabledAsync(IidUser user)
        {
            return Task.Factory.StartNew<bool>(() => false);
        }

        public Task<IList<Claim>> GetClaimsAsync(IidUser user)
        {
            return Task.FromResult((IList<Claim>)user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList());
        }

        public Task AddClaimAsync(IidUser user, Claim claim)
        {
            if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
                user.Claims.Add(new IidUserClaim() { ClaimType = claim.Type, ClaimValue = claim.Value });

            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(IidUser user, Claim claim)
        {
            user.Claims.RemoveAll(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
            return Task.FromResult(0);

        }
    }
}