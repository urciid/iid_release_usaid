using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Helpers;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public enum SyncType { Note, Observation, Change, Attachment, Comment }

    public enum SyncAction { Upsert, Delete }

    public class SyncLog : Base
    {
        public SyncLog(SyncAction action, SyncType type, dynamic item)
        {
            Action = action;
            Type = type;
            Item = item;
        }

        public SyncLog(SyncAction action, SyncType type, dynamic item, string message)
        {
            Action = action;
            Type = type;
            Item = item;
            Successful = false;
            Message = message;
        }

        public SyncType Type { get; private set; }
        public SyncAction Action { get; private set; }
        public object Item { get; private set; }
        public bool Successful { get; private set; }
        public int? NewEntityId { get; private set; }
        public string Message { get; private set; }
        public string IndicatorName { get; private set; }
        public string SiteName { get; private set; }
        public DateTime Date { get; private set; }

        public void SetSuccessful(int? newEntityId)
        {
            Successful = true;
            NewEntityId = newEntityId;
        }

        public void SetFailed(string message)
        {
            Successful = false;
            Message = message;
        }

        public void SetObservation(int observationId)
        {
            var observation = Context.t_observation.Find(observationId);
            if (observation != null)
            {
                IndicatorName = observation.indicator.get_name_translated(CurrentLanguageId);
                SiteName = observation.site.name;
                Date = observation.begin_date;
            }
        }

        public void SetObservation(int indicatorId, int siteId, DateTime beginDate)
        {
            var indicator = Context.t_indicator.Find(indicatorId);
            var site = Context.t_site.Find(siteId);
            if (indicator != null && site != null)
            {
                IndicatorName = indicator.get_name_translated(CurrentLanguageId);
                SiteName = site.name;
                Date = beginDate;
            }
        }
    }

    public class OnlineSync : Base
    {
        private OnlineSync() { }

        public static IEnumerable<SyncLog> SyncChanges(string serializedItems)
        {
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(serializedItems);
            var instance = new OnlineSync();
            var logs = new List<SyncLog>();
            int observationId;

            const string parentObservationErrorMessage = "Parent observation was not saved.";

            // Capture new observation ID's so that child objects can be mapped to them.
            Dictionary<string, int> newObservations = new Dictionary<string, int>();
            foreach (var o in items.Observations.Upserts)
            {
                SyncLog log = instance.Upsert(SyncType.Observation, (object)o, instance.UpsertObservation);
                if (log.NewEntityId.HasValue)
                    newObservations.Add((string)o.ObservationId, log.NewEntityId.Value);
                log.SetObservation(log.NewEntityId ?? (int)o.ObservationId);
                logs.Add(log);
            }

            // Observation Changes
            foreach (var c in items.ObservationChanges.Upserts)
            {
                // If the change belongs to a newly-inserted observation, update ObservationId first.
                if (!Int32.TryParse((string)c.ObservationId, out observationId))
                {
                    if (newObservations.ContainsKey((string)c.ObservationId))
                    {
                        // Set the new ObservationId.
                        c.ObservationId = newObservations[(string)c.ObservationId];
                    }
                    else
                    {
                        // Could not find the ObservationId. Do not import the change.
                        logs.Add(new SyncLog(SyncAction.Upsert, SyncType.Change, c, parentObservationErrorMessage));
                        continue;
                    }
                }

                var log = instance.Upsert(SyncType.Change, (object)c, instance.UpsertChange);
                log.SetObservation((int)c.ObservationId);
                logs.Add(log);
            }
            foreach (var c in items.ObservationChanges.Deletes)
            {
                logs.Add(instance.Delete(SyncType.Change, (int)c, instance.DeleteChange));
            }

            // Observation Attachments
            foreach (var a in items.ObservationAttachments.Upserts)
            {
                // If the attachment belongs to a newly-inserted observation, update ObservationId first.
                if (!Int32.TryParse((string)a.ObservationId, out observationId))
                {
                    if (newObservations.ContainsKey((string)a.ObservationId))
                    {
                        // Set the new ObservationId.
                        a.ObservationId = newObservations[(string)a.ObservationId];
                    }
                    else
                    {
                        // Could not find the ObservationId. Do not import the attachment.
                        logs.Add(new SyncLog(SyncAction.Upsert, SyncType.Attachment, a, parentObservationErrorMessage));
                        continue;
                    }
                }

                var log = instance.Upsert(SyncType.Attachment, (object)a, instance.UpsertAttachment);
                log.SetObservation((int)a.ObservationId);
                logs.Add(log);
            }

            // Observation Comments
            foreach (var c in items.ObservationComments.Upserts)
            {
                // If the comment belongs to a newly-inserted observation, update ObservationId first.
                if (!Int32.TryParse((string)c.ObservationId, out observationId))
                {
                    if (newObservations.ContainsKey((string)c.ObservationId))
                    {
                        // Set the new ObservationId.
                        c.ObservationId = newObservations[(string)c.ObservationId];
                    }
                    else
                    {
                        // Could not find the ObservationId. Do not import the comment.
                        logs.Add(new SyncLog(SyncAction.Upsert, SyncType.Comment, c, parentObservationErrorMessage));
                        continue;
                    }
                }

                var log = instance.Upsert(SyncType.Comment, (object)c, instance.UpsertComment);
                log.SetObservation((int)c.ObservationId);
                logs.Add(log);
            }
            foreach (var c in items.ObservationComments.Deletes)
            {
                logs.Add(instance.Delete(SyncType.Comment, (int)c, instance.DeleteComment));
            }

            // If any item(s) failed, send a copy of the raw data to the system adminsitrator(s).
            if (logs.Any(x => !x.Successful))
                SendLogEmail(logs);

            return logs;
        }
        
        private static readonly DateTime BaseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static void SendLogEmail(IEnumerable<SyncLog> logs)
        {
            string body = (
                "<html>\r\n" +
                "<body>\r\n" +
                "<p>An error occurred importing Disconnected Mode data:</p>\r\n" +
                "<table>\r\n" +
                "   <tr><td>Name:</td><td>" + Identity.CurrentUser.FullName + "</td></tr>\r\n" +
                "   <tr><td>Email:</td><td>" + Identity.CurrentUser.UserName + "</td></tr>\r\n" +
                "   <tr><td>Time:</td><td>" + DateTime.Now.ToString("M/d/yyyy h:mm") + "</td></tr>\r\n" +
                "</table>\r\n" +
                "<br />\r\n" +
                "<table border=\"1\" cellpadding=\"4\" style=\"border-collapse: collapse;\">\r\n" +
                "   <tr style=\"background-color: #597790; color: #FFFFFF; font-weight: bold;\">\r\n" +
                "       <td nowrap>Indicator</td>\r\n" +
                "       <td nowrap>Site</td>\r\n" +
                "       <td nowrap>Date</td>\r\n" +
                "       <td nowrap>Type</td>\r\n" +
                "       <td nowrap>Successful</td>\r\n" +
                "       <td>Message</td>\r\n" +
                "       <td>Raw Data</td>\r\n" +
                "   </tr>\r\n");
            int i = 0;
            foreach (var log in logs)
            {
                string rowBackgroundRgb = i % 2 == 0 ? "FFFFFF" : "F9F9F9";
                string successRgb = log.Successful ? "609030" : "C00C03";
                body += (
                    "   <tr style=\"background-color: #" + rowBackgroundRgb  + ";\">\r\n" +
                    "       <td>" + (log.IndicatorName ?? "Unknown")  + "</td>\r\n" +
                    "       <td nowrap>" + (log.SiteName ?? "Unknown") + "</td>\r\n" +
                    "       <td nowrap>" + log.Date.ToString("M/d/yyyy") + "</td>\r\n" +
                    "       <td nowrap>" + log.Type.ToString() + "</td>\r\n" +
                    "       <td nowrap><b style=\"color: #" + successRgb + ";\">" + log.Successful + "</b></td>\r\n" +
                    "       <td>" + log.Message + "</td>\r\n" +
                    "       <td>" + JsonConvert.SerializeObject(log.Item).Replace("},", "},<br />") + "</td>\r\n" +
                    "   </tr>\r\n");
                i++;
            }
            body += (
                "</table>\r\n" + 
                "</body>\r\n" +
                "</html>");

            SendGrid.SendEmail(Settings.AdminEmailAddress, "IID Disconnected Mode Data Sync Failed", body, true);
        }

        #region Upsert Members

        private delegate int? UpsertModel(dynamic item);

        private SyncLog Upsert(SyncType type, object item, UpsertModel upsertDelegate)
        {
            var log = new SyncLog(SyncAction.Upsert, type, item);
            try
            {
                int? newEntityId = upsertDelegate(item);
                log.SetSuccessful(newEntityId);
            }
            catch (Exception ex)
            {
                log.SetFailed(ex.Message);
            }
            return log;
        }

        private int? UpsertObservation(dynamic model)
        {
            int indicatorId = (int)model.IndicatorId;
            int siteId = (int)model.SiteId;
            DateTime beginDate = Convert.ToDateTime(model.BeginDate).Date;

            // Check if the observation already exists. There are a few scenarios:
            // 1) New observation which does not exist.
            // 2) "New" observation which has been created since user went into disconnected mode.
            // 3) Existing observation being updated.
            // 4) "Existing" observation which no longer exists for some reason. (Shouldn't happen, but who knows?)
            t_observation entity = Context.t_observation.Where(x => x.indicator_id == indicatorId && x.site_id == siteId && x.begin_date == beginDate).FirstOrDefault();
            if (entity == null)
            {
                entity = new t_observation()
                {
                    indicator_id = indicatorId,
                    site_id = siteId,
                    begin_date = beginDate,
                    end_date = Convert.ToDateTime(model.EndDate).Date,
                    createdby_userid = CurrentUserId,
                    created_date = Convert.ToDateTime(model.CreatedOn),
                    is_age_disaggregated = model.IsAgeDisaggregated,
                    is_sex_disaggregated = model.IsSexDisaggregated
                };
                Context.t_observation.Add(entity);
            }
            else
            {
                entity.updatedby_userid = CurrentUserId;
                entity.updated_date = Convert.ToDateTime(model.CreatedOn);
            }
            Context.SaveChanges();

            // Load the indicator. Needed for parsing entry records.
            Context.Entry(entity).Reference(e => e.indicator).Load();
            string indicatorType = entity.indicator.indicator_type_fieldid;

            // Wipe out pre-existing entry values. We can't just delete them because of the audit trail.
            var entries = new Dictionary<string, t_observation_entry>();
            foreach (var entry in entity.entries)
            {
                entry.numerator = null;
                entry.denominator = null;
                entry.count = null;
                entry.rate = null;
                entry.yes_no = null;
                entries.Add(String.Format("{0}|{1}", entry.indicator_age_range_id, entry.indicator_gender), entry);
            }

            // Parse model entries. These may or may not already existing in the db (depending on disaggregation options).
            foreach (var modelEntry in model.EntriesCollection)
            {
                string key = String.Format("{0}|{1}", modelEntry.Value.age_range_id, modelEntry.Value.sex_code);
                t_observation_entry entry = null;
                if (entries.ContainsKey(key))
                {
                    entry = entries[key];
                }
                else
                {
                    entry = new t_observation_entry()
                    {
                        observation_id = entity.observation_id,
                        indicator_age_range_id = modelEntry.Value.age_range_id,
                        indicator_gender = modelEntry.Value.sex_code,
                        createdby_userid = CurrentUserId,
                        created_date = Convert.ToDateTime(model.CreatedOn)
                    };
                    entries.Add(key, entry); // dictionary
                    entity.entries.Add(entry); // database
                }
                switch ((string)modelEntry.Value.type)
                {
                    case "numerator":
                        entry.numerator = modelEntry.Value.value;
                        break;

                    case "denominator":
                        entry.denominator = modelEntry.Value.value;
                        break;

                    default:
                        switch (indicatorType)
                        {
                            case "indcnt":
                                entry.count = modelEntry.Value.value;
                                break;

                            case "indrat":
                                entry.rate = modelEntry.Value.value;
                                break;

                            case "indyes":
                                entry.yes_no = model.value;
                                break;
                        }
                        break;
                }
            }
            Context.SaveChanges();

            // Different logic than the other upserts because of collision scenarios.
            // If the ID has changed for some reason, return the new ID so that child 
            // objects (changes, attachments, comments) can be mapped to it.
            if (model.ObservationId.ToString() == entity.observation_id.ToString())
                return null;
            else
                return entity.observation_id;
        }

        private int? UpsertChange(dynamic model)
        {

            // "New" changes are always new (no collisions). Check if "updated" changes 
            // exist before updating; if they no longer exist for some reason, re-create.
            t_observation_change entity = null;
            int changeId;
            if (Int32.TryParse(model.ChangeId.ToString(), out changeId))
                entity = Context.t_observation_change.Find(changeId);
            bool isNew = entity == null;

            if (isNew)
            {
                entity = new t_observation_change()
                {
                    observation_id = model.ObservationId,
                    approved = false,
                    createdby_userid = CurrentUserId,
                    created_date = Convert.ToDateTime(model.CreatedOn)
                };
                Context.t_observation_change.Add(entity);
            }
            else
            {
                entity.updatedby_userid = CurrentUserId;
                entity.updated_date = Convert.ToDateTime(model.UpdatedOn);
            }
            entity.start_date = Convert.ToDateTime(model.StartDate);
            entity.description = model.Description;
            Context.SaveChanges();

            if (isNew)
                return entity.observation_change_id;
            else
                return null;
        }

        private int? UpsertAttachment(dynamic model)
        {
            // Offline attachments are always treated as new attachments. (We don't 
            // sync existing attachments to localStorage because of storage limitations).
            var base64 = (string)model.Attachment;
            var bytes = Convert.FromBase64String(base64.Substring(base64.IndexOf(',') + 1));
            t_observation_attachment entity = new t_observation_attachment()
            {
                observation_id = model.ObservationId,
                attachment = bytes,
                attachment_file_name = model.FileName,
                active = false,
                createdby_userid = CurrentUserId,
                created_date = Convert.ToDateTime(model.CreatedOn)
            };
            Context.t_observation_attachment.Add(entity);
            Context.SaveChanges();

            return entity.observation_attachment_id;
        }

        private int? UpsertComment(dynamic model)
        {

            // "New" comments are always new (no collisions). Check if "updated" comments 
            // exist before updating; if they no longer exist for some reason, re-create.
            t_observation_comment entity = null;
            int commentId;
            if (Int32.TryParse(model.CommentId.ToString(), out commentId))
                entity = Context.t_observation_comment.Find(commentId);
            bool isNew = entity == null;

            if (isNew)
            {
                entity = new t_observation_comment()
                {
                    observation_id = model.ObservationId,
                    createdby_userid = CurrentUserId,
                    created_date = Convert.ToDateTime(model.CreatedOn)
                };
                Context.t_observation_comment.Add(entity);
            }
            else
            {
                entity.updatedby_userid = CurrentUserId;
                entity.updated_date = Convert.ToDateTime(model.UpdatedOn);
            }
            entity.comment = model.Comment;
            Context.SaveChanges();

            if (isNew)
                return entity.observation_comment_id;
            else
                return null;
        }

        #endregion

        #region Delete Members

        private delegate void DeleteModel(int id);

        private SyncLog Delete(SyncType type, int id, DeleteModel deleteDelegate)
        {
            var log = new SyncLog(SyncAction.Delete, type, id);
            try
            {
                deleteDelegate(id);
                log.SetSuccessful(null);
            }
            catch (Exception ex)
            {
                log.SetFailed(ex.Message);
            }
            return log;
        }

        private void DeleteEntity<TEntity>(int id) where TEntity : class
        {
            TEntity entity = Context.Set<TEntity>().Find(id);
            if (entity != null)
            {
                Context.Set<TEntity>().Remove(entity);
                Context.SaveChanges();
            }
        }

        private void DeleteNote(int noteId)
        {
            DeleteEntity<t_note>(noteId);
        }

        private void DeleteChange(int changeId)
        {
            DeleteEntity<t_observation_change>(changeId);
        }

        private void DeleteComment(int commentId)
        {
            DeleteEntity<t_observation_comment>(commentId);
        }

        #endregion

        #region Utility Members

        private DateTime ConvertIdToDate(dynamic id)
        {
            return GetDateTimeFromSeconds(Convert.ToInt64(((string)id).Substring(1)));
        }

        private DateTime GetDateTimeFromSeconds(long seconds)
        {
            return BaseDate.AddMilliseconds(seconds);
        }

        #endregion
    }
}