using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IID.BusinessLayer.Domain;

namespace IID.BusinessLayer.Identity
{

    public enum PermissionType : byte
    {
        All = 0,
        Activity = 1,
        Country = 2,
        AdministrativeDivision = 3,
        Site = 4
    }

    public class Permission
    {
        private Permission() { }

        [Key]
        [Column(Order = 0)]
        public byte permission_type_id { get; private set; }
        public PermissionType PermissionType { get { return (PermissionType)permission_type_id; } }

        private int object_id { get; set; }
        [Key]
        [Column(Order = 0)]
        public int ObjectId { get { return object_id; } }

        private bool view_access { get; set; }
        public bool ViewAccess { get { return view_access; } }

        private bool update_access { get; set; }
        public bool UpdateAccess { get { return update_access; } }

        public static ICollection<Permission> GetPermissions(int userId, PermissionType permissionType)
        {
            var parameters = new Dictionary<string, object>() { { "user_id", userId }, { "permission_type", (byte)permissionType } };
            return StoredProcedures.GetEntities<Permission>("dbo.p_get_user_permissions", parameters);
        }
    }
}
