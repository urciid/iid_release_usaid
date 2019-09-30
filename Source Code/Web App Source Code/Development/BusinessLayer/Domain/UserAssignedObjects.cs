using System.Collections.Generic;
using System.Linq;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Identity;

namespace IID.BusinessLayer.Domain
{
    public static class UserAssignedObjects
    {
        public static IEnumerable<t_project> GetProjects(int userId, int languageId)
        {
            var parameters = new Dictionary<string, object>() { { "user_id", userId }, { "language_id", languageId } };
            return StoredProcedures.GetEntities<t_project>("dbo.p_get_projects", parameters);
        }

        public static IEnumerable<t_country> GetCountries(int userId, int languageId)
        {
            var parameters = new Dictionary<string, object>() { { "user_id", userId }, { "language_id", languageId } };
            return StoredProcedures.GetEntities<t_country>("dbo.p_get_countries", parameters);
        }

        public static IEnumerable<t_aim> GetAims(IEnumerable<t_aim> aims, IidUser user)
        {
            return aims.Where(e =>
                        e.active == true ||
                        user.IsInRole(Role.SystemAdministrator) ||
                        e.createdby_userid == user.Id
                    ) // TODO: Security check on aims => activities.
                    .OrderBy(e => e.sort);
        }

        public static IEnumerable<t_indicator> GetIndicators(IEnumerable<t_indicator> indicators, IidUser user)
        {
            return indicators.Where(e =>
                        e.active == true ||
                        user.IsInRole(Role.SystemAdministrator) ||
                        e.createdby_userid == user.Id
                    ) // TODO: Security check on indicators => aims => activities.
                    .OrderBy(e => e.sort);
        }
    }
}