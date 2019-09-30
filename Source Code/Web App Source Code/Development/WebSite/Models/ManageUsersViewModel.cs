using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using common = IID.BusinessLayer.Globalization.Common.Resource;
using site = IID.BusinessLayer.Globalization.Site.Resource;
using IID.BusinessLayer.Helpers;
using System.Linq;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class ManageUsersViewModel
    {
        public ManageUsersViewModel() { }

        public static ManageUsersViewModel FromDatabase(ICollection<v_user> users)
        {
            ManageUsersViewModel instance = new ManageUsersViewModel();
            instance.AllUsers = users;            
            return instance;
        }


        [Display(Name = "All Users", ResourceType = typeof(common))]
        public ICollection<v_user> AllUsers { get; set; }




    }
}