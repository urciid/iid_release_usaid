using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.BusinessLayer.Helpers;
using System.Linq;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{

    public class FieldIDViewModel
    {

        public FieldIDViewModel() { }

        public static FieldIDViewModel FromDatabase(string ParentFieldID, string ParentFieldIDValue, ICollection<FieldIDModel> fieldids)
        {
            FieldIDViewModel instance = new FieldIDViewModel();
            instance.ParentFieldID = ParentFieldID;
            instance.ParentFieldIDValue = ParentFieldIDValue;
            instance.AllFieldIDs = fieldids;
            
            return instance;
        }


        [Display(Name = "All FieldIDs", ResourceType = typeof(common))]
        public ICollection<FieldIDModel> AllFieldIDs { get; set; }
        public string ParentFieldID { get; set; }

        public string ParentFieldIDValue { get; set; }
    }

    public partial class FieldIDModel
    {
        public string FieldID { get; set; }
        public string ParentFieldID { get; set; }
        public string Value { get; set; }
        public Boolean? Active { get; set; }
        public int? SortKey { get; set; }

    }


}