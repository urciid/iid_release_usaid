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
    
    public partial class t_language
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_language()
        {
            this.t_announcement = new HashSet<t_announcement>();
            this.t_translation = new HashSet<t_translation>();
        }
    
        public byte language_id { get; set; }
        public string language { get; set; }
        public byte[] icon { get; set; }
        public string language_code { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_announcement> t_announcement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_translation> t_translation { get; set; }
    }
}
