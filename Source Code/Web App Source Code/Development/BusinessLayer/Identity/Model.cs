using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IID.BusinessLayer.Identity
{

    public class IdentityModel : DbContext
    {
        public IdentityModel()
            : base("Identity")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        public static IdentityModel Create()
        {
            return new IdentityModel();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IidUser>()
                .Property(e => e.SecurityStamp)
                .IsUnicode(false);

            modelBuilder.Entity<IidUser>()
                .HasMany(e => e.UserRoles)
                .WithRequired(e => e.t_user)
                .HasForeignKey(e => e.user_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IidRole>()

                .Property(e => e.user_role_fieldid)
                .IsUnicode(false);
        }

        public virtual IDbSet<IidUser> Users { get; set; }
        public virtual IDbSet<IidRole> Roles { get; set; }
    }
}