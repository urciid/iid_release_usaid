using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IID.WebSite.Models;
using r = IID.WebSite.Models;
using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class RequestController : BaseController
    {
        // GET: Request
        public ActionResult View(RequestActiveFlags[] flags)
        {

            return View(RequestModel.GetRequestModel(CurrentUser.Id, ConcatenateFlags(flags)));
        }

        public ActionResult Admin(RequestActiveFlags[] flags)
        {
            return View(RequestModel.GetRequestModel(null, ConcatenateFlags(flags)));
        }

        private RequestActiveFlags ConcatenateFlags(RequestActiveFlags[] flags)
        {
            RequestActiveFlags reqFlags = RequestActiveFlags.None;

            if (flags == null)
                reqFlags = RequestActiveFlags.Pending;
            else
                foreach (var flag in flags)
                    reqFlags = (reqFlags | flag);

            return reqFlags;
        }

        public JsonResult Process(int objectTypeId, int objectId, bool active)
        {
            try
            {
                using (Entity db = new Entity())
                {
                    IRequestEntity entity = null;
                    switch (Enumerations.Parse<RequestObjectType>(objectTypeId))
                    {
                        case RequestObjectType.Activity:
                            entity = db.t_activity.Find(objectId);
                            break;

                        case RequestObjectType.Aim:
                            entity = db.t_aim.Find(objectId);
                            break;

                        case RequestObjectType.Country:
                            entity = db.t_country.Find(objectId);
                            break;

                        case RequestObjectType.Indicator:
                            entity = db.t_indicator.Find(objectId);
                            break;

                        case RequestObjectType.Project:
                            entity = db.t_project.Find(objectId);
                            break;

                        case RequestObjectType.Site:
                            entity = db.t_site.Find(objectId);
                            break;

                        case RequestObjectType.User:
                            entity = db.t_user.Find(objectId);
                            break;

                        case RequestObjectType.Attachment:
                            entity = db.t_observation_attachment.Find(objectId);
                            break;
                    }
                    entity.active = active;
                    entity.updatedby_userid = CurrentUser.Id;
                    entity.updated_date = DateTime.Now;
                    db.SaveChanges();
                }

                return GetJsonResult(true, String.Empty);
            }
            catch(Exception ex)
            {
                return GetJsonResult(ex);
            }
        }
    }
}