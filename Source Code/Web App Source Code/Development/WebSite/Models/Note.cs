using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using error = IID.BusinessLayer.Globalization.Error.Resource;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class Note
    {
        public Note() { }

        public Note(int? siteId, int? activityId)
        {
            SiteId = siteId;
            ActivityId = activityId;
        }

        public Note(t_note entity)
        {
            NoteId = entity.note_id;
            SiteId = entity.site_id;
            ActivityId = entity.activity_id;
            Subject = entity.subject;
            NoteText = entity.note_text;
            CreatedByUserId = entity.createdby_userid;
            CreatedOn = entity.created_date;
            UpdatedByUserId = entity.updatedby_userid;
            UpdatedOn = entity.updated_date;
        }

        public int? NoteId { get; set; }

        public int? SiteId { get; set; }

        public int? ActivityId { get; set; }

        [AllowHtml]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [StringLength(100)]
        public string Subject { get; set; }

        [AllowHtml]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        [DataType(DataType.MultilineText)]
        public string NoteText { get; set; }

        public int CreatedByUserId { get; set; }
        public string CreatedBy { get { return UserName.Get(CreatedByUserId); } }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedByUserId { get; set; }
        public string UpdatedBy { get { return UpdatedByUserId.HasValue ? UserName.Get(UpdatedByUserId.Value) : null; } }
        public DateTime? UpdatedOn { get; set; }
    }

    public class ListNotesViewModel
    {
        private ListNotesViewModel() { }

        public ListNotesViewModel(int? siteId, int? activityId, ICollection<Note> notes)
        {
            SiteId = siteId;
            ActivityId = activityId;
            Notes = notes;
        }

        public int? SiteId { get; private set; }

        public int? ActivityId { get; private set; }

        public ICollection<Note> Notes { get; private set; }
    }
}