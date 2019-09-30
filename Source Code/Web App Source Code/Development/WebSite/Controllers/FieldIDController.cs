using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Identity;
using IID.WebSite.Helpers;
using IID.WebSite.Models;
using System.Threading.Tasks;
using static IID.WebSite.Models.FieldIDViewModel;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class FieldIDController : BaseController
    {
        [Authorize(Roles = IidRole.SystemAdministratorRole)]
        public ActionResult Manage(string id)
        {
            // Note: id is a Parent FieldID
            using (Entity db = new Entity())
            {
                t_fieldid parent_fieldid = db.t_fieldid.Where(e => e.fieldid == id).FirstOrDefault();

                List<t_fieldid> tfieldids = db.t_fieldid.Where(e => e.parent_fieldid == id).OrderBy(e => e.value).ToList();

                ICollection<FieldIDModel> fieldids = new List<FieldIDModel>();
                // This is a slight hack but it works - I'm sure there's a more elegant way :)
                foreach (t_fieldid f in tfieldids)
                {
                    FieldIDModel m = new FieldIDModel();
                    m.FieldID = f.fieldid;
                    m.ParentFieldID = f.parent_fieldid;
                    m.Value = f.value;
                    m.SortKey = f.sort_key;
                    m.Active = f.active;
                    fieldids.Add(m);
                }
                
                if (fieldids.Count == 0)
                    throw new ArgumentException("Error getting FieldID entries.");
                
                return View(FromDatabase(id, parent_fieldid.value, fieldids));
            }
        }
        

        public ActionResult Add(string id)
        {
            FieldID addRec = new FieldID();
            addRec.ParentFieldID = id;
            return View(addRec);
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FieldID model)
        {
            if(CurrentUser.IsInRole(Role.SystemAdministrator) && model.Value.Length > 0 && model.ParentFieldID.Length > 0)
            {
                using (Entity db = new Entity())
                {
                    // Set and save user fields.
                    t_fieldid rec = new t_fieldid();
                    db.t_fieldid.Add(rec);
                    rec.fieldid = GetNextKey(model.ParentFieldID);
                    rec.value = model.Value;
                    rec.parent_fieldid = model.ParentFieldID;
                    rec.createdby_userid = CurrentUser.Id;
                    rec.created_date = DateTime.Now;
                    rec.active = true;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Manage", new { id = model.ParentFieldID });
        }

        public ActionResult Edit(string id)
        {
            return View(new FieldID(id));
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FieldID model)
        {
         //   if (ModelState.IsValid)
         //   {
                using (Entity db = new Entity())
                {
                    if (model._FieldID.Length == 0)
                    {
                        throw new ArgumentException("FieldID not provided");
                    }
                    
                    t_fieldid rec = db.t_fieldid.Find(model._FieldID);
                    if (rec == null)
                        throw new ArgumentException("Invalid FieldID: " + model._FieldID);
                                       
                    // Set and save user fields.
                    SetCoreFields(ref rec, model);
                    rec.updatedby_userid = CurrentUser.Id;
                    rec.updated_date = DateTime.Now;
                    db.SaveChanges();                    

                    return RedirectToAction("Manage", new { id = rec.parent_fieldid });
                }
            //}
            //else
            //{
            //    return View(model);
            //}
        }

        private void SetCoreFields(ref t_fieldid rec, FieldID model)
        {
            rec.value = model.Value;
            rec.active = model.Active;            
        }

        private string GetNextKey(string ParentFieldID)
        {
            int x = 0;
            string strSeq = "";
            string newFieldID = "";       
            do
            {
                using (Entity db = new Entity())
                {
                    strSeq = x.ToString("000");
                    newFieldID = ParentFieldID.Substring(0, 3) + strSeq;
                    t_fieldid rec = db.t_fieldid.Where(e => e.fieldid == newFieldID).FirstOrDefault();

                    if (rec == null)  // we have a winner
                    {
                        x = 99999;
                    }
                    else
                    {
                        x++;
                    }

                }
            } while (x < 999);  // this limits at most 999 new inserts for a given parent

            return newFieldID;
        }

      
    }
}