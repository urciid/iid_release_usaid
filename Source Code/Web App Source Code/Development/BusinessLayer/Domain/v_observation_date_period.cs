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
    
    public partial class v_observation_date_period
    {
        public int activity_id { get; set; }
        public int aim_id { get; set; }
        public int indicator_id { get; set; }
        public string data_collection_frequency_fieldid { get; set; }
        public int site_id { get; set; }
        public Nullable<int> observation_id { get; set; }
        public System.DateTime begin_date { get; set; }
        public System.DateTime end_date { get; set; }
        public bool has_data { get; set; }
        public bool has_changecommentattachment { get; set; }
    }
}
