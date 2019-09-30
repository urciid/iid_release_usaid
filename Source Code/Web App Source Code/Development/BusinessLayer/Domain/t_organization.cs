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
    
    public partial class t_organization
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_organization()
        {
            this.projects = new HashSet<t_project>();
            this.users = new HashSet<t_user>();
        }
    
        public int organization_id { get; set; }
        public string name { get; set; }
        public int createdby_userid { get; set; }
        public System.DateTime created_date { get; set; }
        public Nullable<int> updatedby_userid { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
    
        public virtual t_user createdby { get; set; }
        public virtual t_user updatedby { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_project> projects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_user> users { get; set; }
    }
}
