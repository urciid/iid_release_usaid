using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IID.BusinessLayer.Domain;

namespace IID.BusinessLayer.Helpers
{
    public enum AccessLevel { None, View, Update, Any }

    public enum AccessType { Country, Activity, AdministrativeDivision, Site }

    public class UserAccess
    {
        private static Expression<Func<t_user_access, bool>> CheckAccessLevel(AccessLevel level)
        {
            switch (level)
            {
                case AccessLevel.View:
                    return e => e.view_access || e.update_access;

                case AccessLevel.Update:
                    return e => e.update_access;

                case AccessLevel.Any:
                    return e => e.view_access || e.update_access;

                default: // None
                    return e => !(e.view_access || e.update_access);
            }
        }

        private static Expression<Func<t_user_access, int?>> GetAccessId(AccessType type)
        {
            switch (type)
            {
                case AccessType.Country:
                    return e => e.country_id;

                case AccessType.AdministrativeDivision:
                    return e => e.administrative_division_id;

                case AccessType.Site:
                    return e => e.site_id;

                default: // Activity
                    return e => e.activity_id;
            }
        }

        public static ICollection<int?> GetExistingUserAccess(int userId, AccessType type, AccessLevel level)
        {
            using (Entity db = new Entity())
            {
                // We are using accessLevel as a filter, so translate None --> Any.
                if (level == AccessLevel.None)
                    level = AccessLevel.Any;

                return db.t_user_access
                    .Where(e => e.user_id == userId)
                    .Where(CheckAccessLevel(level))
                    .Select(GetAccessId(type))
                    .Where(id => id.HasValue)
                    .Distinct()
                    .ToArray();
            }
        }
    }
}
