//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IID.BusinessLayer.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class v_user
    {
        public int user_id { get; set; }
        public Nullable<int> organization_id { get; set; }
        public Nullable<System.Guid> user_guid { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public Nullable<int> user_status_id { get; set; }
        public int consecutive_failed_login_attempts { get; set; }
        public Nullable<System.DateTime> last_timebox { get; set; }
        public Nullable<int> site_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phone { get; set; }
        public string title { get; set; }
        public int createdby_userid { get; set; }
        public System.DateTime created_date { get; set; }
        public Nullable<int> updatedby_userid { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string security_stamp { get; set; }
        public Nullable<bool> active { get; set; }
        public string user_role_fieldid { get; set; }
        public string user_role_fieldid_value { get; set; }
        public Nullable<System.DateTime> last_login_activity { get; set; }
    }
}
