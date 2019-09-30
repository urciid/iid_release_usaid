using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Models;
using IID.WebSite.Helpers;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class ObservationController : BaseController
    {
        public ActionResult View(int indicatorId, int siteId)
        {
            using (Entity context = new Entity())
            {
                t_indicator indicator = context.t_indicator.Find(indicatorId);
                if (indicator == null)
                    throw new ArgumentException("Invalid indicatorId: " + indicatorId.ToString());

                t_activity activity = indicator.aim.activity;
                t_activity_site site = activity.sites.Where(e => e.site_id == siteId).First();
                if (site == null)
                    throw new ArgumentException(String.Format("Site {0} is not attached to Indicator {1} (Activity {2}).", siteId, indicatorId, activity.activity_id));

                byte languageId = IidCulture.CurrentLanguageId;
                
                IndicatorSiteObservations model = new IndicatorSiteObservations()
                {
                    ActivityId = activity.activity_id,
                    ActivityName = activity.get_name_translated(languageId),
                    IndicatorId = indicatorId,
                    IndicatorType = Enumerations.Parse<IndicatorType>(indicator.indicator_type.value),
                    IndicatorTypeFieldId = indicator.indicator_type_fieldid,
                    SiteId = siteId,
                    IndicatorName = indicator.get_name_translated(languageId),
                    IndicatorDefinition = indicator.get_definition_translated(languageId),
                    NumeratorName = indicator.get_numerator_name_translated(languageId),
                    NumeratorDefinition = indicator.get_numerator_definition_translated(languageId),
                    DenominatorName = indicator.get_denominator_name_translated(languageId),
                    DenominatorDefinition = indicator.get_denominator_definition_translated(languageId),
                    DisaggregateByAge = indicator.disaggregate_by_age,
                    DisaggregateBySex = indicator.disaggregate_by_sex
                };

                AccessPass = SecurityGuard(CurrentUser, activity.country_id, activity.activity_id, siteId);
                if (AccessPass == BusinessLayer.Identity.UserSecurityAccess.NoAccess)
                {
                    return RedirectToAction("NoAccess", "Home");
                }
                ViewBag.AccessRights = AccessPass;

                return View(model);

            }
        }

        public ActionResult Record(int indicatorId, int siteId, DateTime? beginDate)
        {
            using (Entity context = new Entity())
            {
                t_indicator indicator = context.t_indicator.Find(indicatorId);
                if (indicator == null)
                    throw new ArgumentException("Invalid indicatorId: " + indicatorId.ToString());

                t_activity activity = indicator.aim.activity;

                t_activity_site activitySite = activity.sites.Where(e => e.site_id == siteId).First();

                if (activitySite == null)
                    throw new ArgumentException(String.Format("Site {0} is not attached to Indicator {1} (Activity {2}).", siteId, indicatorId, activity.activity_id));
                
                Observation model = new Observation(indicator, activitySite.site, beginDate);

                AccessPass = SecurityGuard(CurrentUser, activity.country_id, activity.activity_id, siteId);
                if (AccessPass == BusinessLayer.Identity.UserSecurityAccess.NoAccess)
                {
                    return RedirectToAction("NoAccess", "Home");
                }
                ViewBag.AccessRights = AccessPass;
                
                return View(model);

            }
        }

        public ActionResult RecordByAttachment(int id)
        {
            using (Entity context = new Entity())
            {              
                t_observation_attachment attachment = context.t_observation_attachment.Find(id);
                if (attachment == null)
                    throw new ArgumentException("Invalid ObservationAttachmentID: " + id.ToString());

                t_observation observation = context.t_observation.Find(attachment.observation_id);
                if (observation == null)
                    throw new ArgumentException("Invalid observationId: " + attachment.observation_id.ToString());

                t_site site = context.t_site.Find(observation.site_id);
                if (site == null)
                    throw new ArgumentException("Invalid siteId: " + observation.site_id.ToString());

                t_indicator indicator = context.t_indicator.Find(observation.indicator_id);
                if (indicator == null)
                    throw new ArgumentException("Invalid indicatorId: " + observation.indicator_id.ToString());
                
                return View("Record", (new Observation(indicator, site, observation.begin_date)));

                //RedirectToAction("Record", (new Observation(observation.site_id, site, observation.begin_date)));
                //return RedirectToAction("Record", new { indicatorId = observation.indicator_id, siteId = observation.site_id, beginDate = observation.begin_date });
                //return Record(observation.indicator_id, observation.site_id, observation.begin_date);
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult Record([System.Web.Http.FromBody] ObservationEntryCollection model)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    bool isNewObservation = false;

                    t_observation observation =
                        context.t_observation.Where(
                            e => e.indicator_id == model.IndicatorId && e.site_id == model.SiteId &&
                            e.begin_date == model.BeginDate).FirstOrDefault();
                    if (observation == null)
                    {
                        isNewObservation = true;
                        observation = new t_observation()
                        {
                            indicator_id = model.IndicatorId,
                            site_id = model.SiteId,
                            begin_date = model.BeginDate,
                            end_date = model.EndDate,
                            createdby_userid = CurrentUser.Id,
                            created_date = DateTime.Now
                        };
                        context.t_observation.Add(observation);
                    }
                    else
                    {
                        observation.updatedby_userid = CurrentUser.Id;
                        observation.updated_date = DateTime.Now;
                    }
                    if ((observation.is_age_disaggregated && !model.IsAgeDisaggregated) ||
                        (observation.is_sex_disaggregated && !model.IsSexDisaggregated))
                    {
                        // Switching from disaggregated to aggregated. Remove pre-existing entries.
                        context.t_observation_entry.RemoveRange(observation.entries);
                    }
                    observation.is_age_disaggregated = model.IsAgeDisaggregated;
                    observation.is_sex_disaggregated = model.IsSexDisaggregated;
                    context.SaveChanges();

                    foreach (ObservationEntry entry in model.Entries)
                    {
                        var oe =
                            context.t_observation_entry.Where(e =>
                               e.observation_id == observation.observation_id &&
                               e.indicator_age_range_id == (entry.AgeRangeId.HasValue ? entry.AgeRangeId : null) &&
                               e.indicator_gender == (entry.SexCode != null ? entry.SexCode : null)).FirstOrDefault();
                        if (oe == null)
                        {
                            oe = new t_observation_entry()
                            {
                                observation_id = observation.observation_id,
                                indicator_age_range_id = entry.AgeRangeId,
                                indicator_gender = entry.SexCode,
                            };
                            context.t_observation_entry.Add(oe);
                        }
                        oe.numerator = entry.Numerator;
                        oe.denominator = entry.Denominator;
                        oe.count = entry.Count;
                        oe.rate = entry.Ratio;
                        oe.yes_no = entry.YesNo;
                        if (isNewObservation || oe.createdby_userid == 0)
                        {
                            oe.createdby_userid = CurrentUser.Id;
                            oe.created_date = DateTime.Now;
                            observation.entries.Add(oe);
                        }
                        if (!isNewObservation)
                        {
                            oe.updatedby_userid = CurrentUser.Id;
                            oe.updated_date = DateTime.Now;
                        }
                    }
                    context.SaveChanges();

                    t_observation_entry firstEntry =
                        context.t_observation_entry.Include(e => e.createdby).Include(e => e.updateby)
                        .Where(e => e.observation_id == observation.observation_id).First();
                    return Json(new
                    {
                        success = true,
                        ObservationId = observation.observation_id,
                        CreatedBy = firstEntry.createdby.full_name,
                        CreatedOn = firstEntry.created_date.ToString("d"),
                        UpdatedBy = firstEntry.updateby?.full_name,
                        UpdatedOn = firstEntry.updated_date?.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        success = false,
                        responseText = "An error occurred: " + ex.ToString()
                    });
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult GetDatePeriods(int indicatorId, int siteId, DateTime date, int periods)
        {
            return Json(new
            {
                success = true,
                DatePeriods = ObservationDatePeriod.GetDatePeriods(indicatorId, siteId, date, periods)
            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult AddChange(ObservationChange model)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_change change = new t_observation_change();
                    change.observation_id = model.ObservationId;
                    change.start_date = model.StartDate;
                    change.description = model.Description;
                    change.createdby_userid = CurrentUser.Id;
                    change.created_date = DateTime.Now;
                    change.approved = true;  // all changes automatically approved
                    context.t_observation_change.Add(change);
                    context.SaveChanges();


                    context.Entry(change).Reference(n => n.createdby).Load();

                    return Json(new
                    {
                        success = true,
                        ChangeId = change.observation_change_id,
                        ObservationId = change.observation_id,
                        StartDate = change.start_date.ToString("d"),
                        Description = change.description,
                        CreatedBy = change.createdby.full_name,
                        CreatedOn = change.created_date.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult EditChange(ObservationChange model)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_change change = context.t_observation_change.Find(model.ChangeId);
                    if (change == null)
                        throw new ArgumentException("Invalid changeId: " + model.ChangeId.ToString());

                    change.start_date = model.StartDate;
                    change.description = model.Description;
                    change.updatedby_userid = CurrentUser.Id;
                    change.updated_date = DateTime.Now;
                    context.SaveChanges();

                    context.Entry(change).Reference(n => n.updatedby).Load();

                    return Json(new
                    {
                        success = true,
                        StartDate = change.start_date.ToString("d"),
                        Description = change.description,
                        UpdatedBy = change.updatedby.full_name,
                        UpdatedOn = change.updated_date.Value.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult ApproveChange(int changeId, bool approve)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_change change = context.t_observation_change.Find(changeId);
                    if (change == null)
                        throw new ArgumentException("Invalid attachmentId: " + changeId.ToString());

                    change.approved = approve;
                    change.updatedby_userid = CurrentUser.Id;
                    change.updated_date = DateTime.Now;
                    context.SaveChanges();

                    context.Entry(change).Reference(n => n.updatedby).Load();

                    return Json(new
                    {
                        success = true,
                        Approved = change.approved,
                        UpdatedBy = change.updatedby.full_name,
                        UpdatedOn = change.updated_date.Value.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteChange(int changeId)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_change change = context.t_observation_change.Find(changeId);
                    if (change != null)
                    {
                        context.t_observation_change.Remove(change);
                        context.SaveChanges();
                    }
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }



        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult AddAttachment([Bind(Exclude = "FileBytes")]ObservationAttachment model, HttpPostedFileBase fileBytes)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_attachment attachment = new t_observation_attachment();
                    attachment.observation_id = model.ObservationId;
                    attachment.attachment_file_name = model.FileName;
                    byte[] file = new byte[fileBytes.ContentLength];
                    int result = fileBytes.InputStream.Read(file, 0, fileBytes.ContentLength);
                    attachment.attachment = file;
                    attachment.createdby_userid = CurrentUser.Id;
                    attachment.created_date = DateTime.Now;
                    context.t_observation_attachment.Add(attachment);
                    context.SaveChanges();


                    context.Entry(attachment).Reference(n => n.createdby).Load();

                    return Json(new
                    {
                        success = true,
                        AttachmentId = attachment.observation_attachment_id,
                        ObservationId = attachment.observation_id,
                        FileName = attachment.attachment_file_name,
                        FileSize = attachment.attachment.Length,
                        Approved = attachment.active,
                        CreatedBy = attachment.createdby.full_name,
                        CreatedOn = attachment.created_date.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult ApproveAttachment(int attachmentId, bool approve)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_attachment attachment = context.t_observation_attachment.Find(attachmentId);
                    if (attachment == null)
                        throw new ArgumentException("Invalid attachmentId: " + attachmentId.ToString());

                    attachment.active = approve;
                    attachment.updatedby_userid = CurrentUser.Id;
                    attachment.updated_date = DateTime.Now;
                    context.SaveChanges();

                    context.Entry(attachment).Reference(n => n.updatedby).Load();

                    return Json(new
                    {
                        success = true,
                        Active = attachment.active,
                        UpdatedBy = attachment.updatedby.full_name,
                        UpdatedOn = attachment.updated_date.Value.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteAttachment(int attachmentId)
        {
            using (Entity context = new Entity())
            {
                try
                {

                    t_observation_attachment attachment = context.t_observation_attachment.Find(attachmentId);
                    context.t_observation_attachment.Remove(attachment);
                    context.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        public ActionResult DownloadAttachmentFile(int attachmentId)
        {
            using (Entity context = new Entity())
            {
                t_observation_attachment attachment = context.t_observation_attachment.Find(attachmentId);
                if (attachment == null)
                {
                    return new HttpNotFoundResult(String.Format("attachmentId: {0}", attachmentId));
                }
                else
                {
                    return File(attachment.attachment, "application/octet-stream", attachment.attachment_file_name);
                }
            }
        }


        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult AddComment(ObservationComment model)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_comment comment = new t_observation_comment();
                    comment.observation_id = model.ObservationId;
                    comment.comment = model.Comment;
                    comment.createdby_userid = CurrentUser.Id;
                    comment.created_date = DateTime.Now;
                    context.t_observation_comment.Add(comment);
                    context.SaveChanges();


                    context.Entry(comment).Reference(n => n.createdby).Load();

                    return Json(new
                    {
                        success = true,
                        CommentId = comment.observation_comment_id,
                        ObservationId = comment.observation_id,
                        Comment = comment.comment,
                        CreatedBy = comment.createdby.full_name,
                        CreatedOn = comment.created_date.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult EditComment(ObservationComment model)
        {
            using (Entity context = new Entity())
            {
                try
                {
                    t_observation_comment comment = context.t_observation_comment.Find(model.CommentId);
                    if (comment == null)
                        throw new ArgumentException("Invalid commentId: " + model.CommentId.ToString());

                    comment.comment = model.Comment;
                    comment.updatedby_userid = CurrentUser.Id;
                    comment.updated_date = DateTime.Now;
                    context.SaveChanges();

                    context.Entry(comment).Reference(n => n.updatedby).Load();

                    return Json(new
                    {
                        success = true,
                        Comment = comment.comment,
                        UpdatedBy = comment.updatedby.full_name,
                        UpdatedOn = comment.updated_date.Value.ToString("d")
                    });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteComment(int commentId)
        {
            using (Entity context = new Entity())
            {
                try
                {

                    t_observation_comment comment = context.t_observation_comment.Find(commentId);
                    context.t_observation_comment.Remove(comment);
                    context.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return GetJsonResult(ex);
                }
            }
        }
    }
}