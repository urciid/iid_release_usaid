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
    
    public partial class t_indicator
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_indicator()
        {
            this.indicator_age_ranges = new HashSet<t_indicator_age_range>();
            this.observations = new HashSet<t_observation>();
        }
    
        public int indicator_id { get; set; }
        public int aim_id { get; set; }
        public string indicator_type_fieldid { get; set; }
        public string group_fieldid { get; set; }
        public string name { get; set; }
        public string definition { get; set; }
        public string numerator_name { get; set; }
        public string numerator_definition { get; set; }
        public string numerator_source { get; set; }
        public string denominator_name { get; set; }
        public string denominator_definition { get; set; }
        public string denominator_source { get; set; }
        public string data_collection_frequency_fieldid { get; set; }
        public string sampling_fieldid { get; set; }
        public string change_variable { get; set; }
        public bool disaggregate_by_sex { get; set; }
        public bool disaggregate_by_age { get; set; }
        public Nullable<decimal> target_performance { get; set; }
        public Nullable<decimal> threshold_good_performance { get; set; }
        public Nullable<decimal> threshold_poor_performance { get; set; }
        public bool increase_is_good { get; set; }
        public string rate_per_fieldid { get; set; }
        public int createdby_userid { get; set; }
        public System.DateTime created_date { get; set; }
        public Nullable<int> updatedby_userid { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public Nullable<bool> active { get; set; }
        public byte sort { get; set; }
        public string report_class_fieldid { get; set; }

        public virtual t_aim aim { get; set; }
        public virtual t_fieldid data_collection_frequency { get; set; }
        public virtual t_fieldid indicator_group { get; set; }
        public virtual t_fieldid indicator_type { get; set; }
        public virtual t_fieldid rate_per { get; set; }
        public virtual t_fieldid report_class { get; set; }
        public virtual t_fieldid sampling { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_indicator_age_range> indicator_age_ranges { get; set; }
        public virtual t_user createdby { get; set; }
        public virtual t_user updatedby { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_observation> observations { get; set; }
    }
}
