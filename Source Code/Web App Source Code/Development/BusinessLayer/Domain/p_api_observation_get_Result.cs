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
    
    public partial class p_api_observation_get_Result
    {
        public int ObservationID { get; set; }
        public string ActivityName { get; set; }
        public string SiteName { get; set; }
        public string IndicatorName { get; set; }
        public string AimName { get; set; }
        public string IndicatorType { get; set; }
        public string NumeratorName { get; set; }
        public string NumeratorValue { get; set; }
        public string DenominatorName { get; set; }
        public string DenominatorValue { get; set; }
        public string IndicatorValue { get; set; }
        public string DissaggregateByGender { get; set; }
        public string DissaggregateByAge { get; set; }
        public int DatePeriodID { get; set; }
        public string DatePeriodFromDate { get; set; }
        public string DatePeriodToDate { get; set; }
        public string Changes { get; set; }
        public string Attachments { get; set; }
        public string Comments { get; set; }
    }
}
