using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IID.BusinessLayer.Identity
{
    [Table("t_user")]
    public class IidUser : IUser<int>
    {
        private IidUser()
        {
            UserRoles = new HashSet<IidRole>();
            Claims = new List<IidUserClaim>();
        }

        public IidUser(string userName) : this()
        {
            UserName = userName;
        }

        [Key]
        [Column("user_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("email")]
        public string UserName { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        public string FullName {  get { return String.Concat(FirstName, " ", LastName); } }

        [Column("user_guid")]
        public Guid? UserGuid { get; set; }

        [Column("security_stamp")]
        public string SecurityStamp { get; set; }

        [Column("consecutive_failed_login_attempts")]
        public int ConsecutiveFailedLoginAttempts { get; set; }

        [Column("last_timebox")]
        public DateTime? LastTimeBox { get; set; }

        [Column("last_login_activity")]
        public DateTime? LastLoginActivity { get; set; }

        [Column("active")]
        public bool? Active { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IidRole> UserRoles { get; set; }

        public bool IsInRole(Role role)
        {
            return System.Web.HttpContext.Current.User.IsInRole(IidRole.GetRoleCode(role));
        }

        public bool IsInRole(params Role[] roles)
        {
            foreach (Role role in roles)
            {
                if (System.Web.HttpContext.Current.User.IsInRole(IidRole.GetRoleCode(role)))
                    return true;
            }
            return false;
        }

        public List<IidUserClaim> Claims { get; private set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IidUser, int> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("ConsecutiveFailedLoginAttempts", ConsecutiveFailedLoginAttempts.ToString()));
            userIdentity.AddClaim(new Claim("Id", Id.ToString()));
            userIdentity.AddClaim(new Claim("FirstName", FirstName));
            userIdentity.AddClaim(new Claim("LastName", LastName));
            foreach(IidRole role in UserRoles)
                userIdentity.AddClaim(new Claim(ClaimTypes.Role, role.user_role_fieldid));
            return userIdentity;
        }

        public ICollection<Permission> Permissions
        {
            get
            {
                var session = System.Web.HttpContext.Current.Session;
                if (session["Permissions"] == null)
                {
                    session["Permissions"] = Permission.GetPermissions(Id, PermissionType.All);
                }

                return (ICollection<Permission>)session["Permissions"];
            }
        }
    }

    public class IidUserClaim
    { 
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public string ClaimType { get; set; } 
        public string ClaimValue { get; set; } 
    }

    public enum Role
    {
        SystemAdministrator,
        ActivityLeader,
        Coach,
        CountryDirector,
        Funder,
        SiteManager,
        Partner,
        OtherStaff
    }

    public enum UserSecurityAccess
    {
        Update,
        ViewOnly,
        NoAccess
    }
}