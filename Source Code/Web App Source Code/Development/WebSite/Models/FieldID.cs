using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using error = IID.BusinessLayer.Globalization.Error.Resource;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class FieldID
    {

        public FieldID()
        {
           
        }
        public FieldID(string ParentFieldID, int Active)
        {
            this.ParentFieldID = ParentFieldID;
            this.Active = true;
        }

        public FieldID(string fieldid)
        {
            using (Entity db = new Entity())
            {
                t_fieldid item = db.t_fieldid.Find(fieldid);

                if (item == null)
                    throw new ArgumentException("Invalid FieldID: " + fieldid);

                _FieldID = item.fieldid;
                ParentFieldID = item.parent_fieldid;
                Value = item.value;
                if (item.active == true)
                {
                    Active = true;
                }
                else
                {
                    Active = false;
                }
                
                SortKey = item.sort_key;

                CreatedByUserId = item.createdby_userid;
                CreatedDate = item.created_date;
                UpdatedDate = item.updated_date;
                UpdatedByUserId = item.updatedby_userid;
                                
            }
        }


        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(6)]
        [Display(Name = "FieldID")]
        public string _FieldID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(6)]
        public string ParentFieldID { get; set; }        
        public string Value { get; set; }        
        public int CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedByUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? SortKey { get; set; }
        [UIHint("Active")]
        [Display(Name = "Active")]
        public bool Active { get; set; }        
        
    }
}