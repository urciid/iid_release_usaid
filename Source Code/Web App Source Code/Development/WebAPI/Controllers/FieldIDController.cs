using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Http;
using IID.BusinessLayer.Domain;

namespace IID.WebAPI.Controllers
{
    public class FieldIDController :  ApiController
    {

        
        public IEnumerable<p_api_fieldid_get_Result> Get(string parent, string userguid, string languageid)
        {
            //CheckCallerId();

            var context = new Entity();
             
            IEnumerable<p_api_fieldid_get_Result> results = context.p_api_fieldid_get("", parent, userguid);

            return results;             

        }


        [System.Web.Http.Authorize()]
        public p_api_fieldid_get_Result GetByID(string id, string userguid, string languageid)
        {
            var context = new Entity();
            p_api_fieldid_get_Result result = context.p_api_fieldid_get(id, "", userguid).First();
            return result;
        }
    }
}