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
    
    public partial class p_api_get_mydata_indicators_new_Result
    {
        public int IndicatorId { get; set; }
        public string IndicatorType { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Frequency { get; set; }
        public string Aim { get; set; }
        public string Activity { get; set; }
        public string NumeratorName { get; set; }
        public string NumeratorDefinition { get; set; }
        public string DenominatorName { get; set; }
        public string DenominatorDefinition { get; set; }
        public bool DisaggregateBySex { get; set; }
        public bool DisaggregateByAge { get; set; }
    }
}
