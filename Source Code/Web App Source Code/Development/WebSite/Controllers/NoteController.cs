using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.WebSite.Models;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class NoteController : BaseController
    {
        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Add(Note model)
        {
            if (ModelState.IsValid)
            {
                using (Entity db = new Entity())
                {
                    try
                    {
                        if (model.ActivityId.HasValue)
                        {
                            t_activity activity = db.t_activity.Find(model.ActivityId);
                            if (activity == null)
                                throw new ArgumentException("Invalid activityId: " + model.ActivityId.ToString());
                        }

                        if (model.SiteId.HasValue)
                        {
                            t_site site = db.t_site.Find(model.SiteId);
                            if (site == null)
                                throw new ArgumentException("Invalid siteId: " + model.SiteId.ToString());
                        }

                        t_note note = new t_note();
                        note.site_id = model.SiteId;
                        note.activity_id = model.ActivityId;
                        note.subject = model.Subject;
                        note.note_text = model.NoteText;
                        note.createdby_userid = CurrentUser.Id;
                        note.created_date = DateTime.Now;
                        db.t_note.Add(note);
                        db.SaveChanges();

                        db.Entry(note).Reference(n => n.createdby).Load();

                        return GetJsonResult(note);
                    }
                    catch (Exception ex)
                    {
                        return GetJsonResult(false, "An error occurred: " + ex.ToString());
                    }
                }
            }
            else
            {
                return GetJsonResult(false, "Model is invalid.");
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Note model)
        {
            if (ModelState.IsValid)
            {
                using (Entity db = new Entity())
                {
                    try
                    {
                        t_note note = db.t_note.Find(model.NoteId);
                        if (note == null)
                            throw new ArgumentException("Invalid noteId: " + model.NoteId.ToString());

                        note.subject = model.Subject;
                        note.note_text = model.NoteText;
                        note.updatedby_userid = CurrentUser.Id;
                        note.updated_date = DateTime.Now;
                        db.Entry(note).Property(n => n.site_id).IsModified = false;
                        db.Entry(note).Property(n => n.activity_id).IsModified = false;
                        db.Entry(note).Property(n => n.createdby_userid).IsModified = false;
                        db.Entry(note).Property(n => n.created_date).IsModified = false;
                        db.SaveChanges();

                        db.Entry(note).Reference(n => n.updatedby).Load();

                        return GetJsonResult(note);
                    }
                    catch (Exception ex)
                    {
                        return GetJsonResult(false, "An error occurred: " + ex.ToString());
                    }
                }
            }
            else
            {
                return GetJsonResult(false, "Model is invalid.");
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            using (Entity db = new Entity())
            {
                try
                {

                    t_note note = db.t_note.Find(id);
                    db.t_note.Remove(note);
                    db.SaveChanges();
                    return GetJsonResult(true, null);
                }
                catch (Exception ex)
                {
                    return GetJsonResult(false, "An error occurred: " + ex.ToString());
                }
            }
        }

        private JsonResult GetJsonResult(t_note note)
        {
            return Json(new
            {
                success = true,
                NoteId = note.note_id,
                SiteId = note.site_id,
                ActivityId = note.activity_id,
                Subject = note.subject,
                NoteText = note.note_text,
                CreatedBy = note.createdby.full_name,
                CreatedOn = note.created_date.ToString("d"),
                UpdatedBy = note.updatedby?.full_name,
                UpdatedOn = note.updated_date?.ToString("d")
            });
        }
    }
}