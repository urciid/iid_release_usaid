using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IID.BusinessLayer.Identity
{
    [Table("t_user_role")]
    public class IidRole
    {
        public const string SystemAdministratorRole = "roladm";

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string user_role_fieldid { get; set; }

        public virtual IidUser t_user { get; set; }

        public static Role GetRole(string fieldId)
        {
            switch (fieldId)
            {
                case "roladm":
                    return Role.SystemAdministrator;

                case "rolatl":
                    return Role.ActivityLeader;

                case "rolcoa":
                    return Role.Coach;

                case "rolcod":
                    return Role.CountryDirector;

                case "colfun":
                    return Role.Funder;

                case "rolmgr":
                    return Role.SiteManager;

                case "rolpar":
                    return Role.Partner;

                default:
                    return Role.OtherStaff;
            }
        }

        public static string GetRoleCode(Role role)
        {
            switch (role)
            {
                case Role.SystemAdministrator:
                    return "roladm";

                case Role.ActivityLeader:
                    return "rolatl";

                case Role.Coach:
                    return "rolcoa";

                case Role.CountryDirector:
                    return "rolcod";

                case Role.Funder:
                    return "rolfun";

                case Role.SiteManager:
                    return "rolmgr";

                case Role.Partner:
                    return "rolpar";

                default:
                    return "roloth";
            }
        }
    }
}
