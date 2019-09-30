using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using IID.BusinessLayer.Domain;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public enum RequestObjectType
    {
        Project,
        Activity,
        Aim,
        Indicator,
        Country,
        Site,
        User,
        Attachment
    }

    [Flags]
    public enum RequestActiveFlags
    {
        None = 0,
        Pending = 1 << 0,
        Approved = 1 << 1,
        Denied = 1 << 2,
        All = (1 << 3) - 1 // Combine all previous values.
    }

    public class Request
    {
        public RequestObjectType ObjectType { get; set; }
        public string Type {  get { return ObjectType.ToString(); } }
        public int ObjectId { get; set; }

        public string Name { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string LastUpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public bool? Active { get; set; }
        //public string Status
        //{
        //    get
        //    {
        //        switch (Active)
        //        {
        //            case true:
        //                return "Approved";
        //            case false:
        //                return "Denied";
        //            default:
        //                return "Pending";
        //        }
        //    }
        //}
        public string Status { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="flag">NOTE: The boolean flag is just to differentiate this overload from others with inherited types.</param>
        private Request(IRequestEntity entity, bool flag)
        {
            CreatedBy = entity.createdby.full_name;
            CreatedOn = entity.created_date;
            if (entity.updatedby == null)
            {
                LastUpdatedBy = entity.createdby.full_name;
            }
            else
            {
                LastUpdatedBy = entity.updatedby.full_name;
            }
            if (entity.updated_date == null)
            {
                UpdatedOn = entity.created_date;
            }
            else
            {
                UpdatedOn = entity.updated_date;
            }

        }

        public Request(t_activity entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.Activity;
            ObjectId = entity.activity_id;
            Name = entity.get_name_translated(IidCulture.CurrentLanguageId);
            Status = SetStatus(entity.active);            
            
        }

        public Request(t_aim entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.Aim;
            ObjectId = entity.aim_id;
            Name = entity.get_name_translated(IidCulture.CurrentLanguageId);
            Status = SetStatus(entity.active);

        }

        public Request(t_country entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.Country;
            ObjectId = entity.country_id;
            Name = entity.get_name_translated(IidCulture.CurrentLanguageId);
            Status = SetStatus(entity.active);
        }

        public Request(t_indicator entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.Indicator;
            ObjectId = entity.indicator_id;
            Name = entity.get_name_translated(IidCulture.CurrentLanguageId);
            Status = SetStatus(entity.active);
        }

        public Request(t_project entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.Project;
            ObjectId = entity.project_id;
            Name = entity.get_name_translated(IidCulture.CurrentLanguageId);
            Status = SetStatus(entity.active);

        }

        public Request(t_site entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.Site;
            ObjectId = entity.site_id;
            Name = entity.name;
            Status = SetStatus(entity.active);
        }

        public Request(t_user entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.User;
            ObjectId = entity.user_id;
            Name = entity.full_name;
            Status = SetStatus(entity.active);
        }
        public Request(t_observation_attachment entity) : this(entity, false)
        {
            ObjectType = RequestObjectType.Attachment;
            ObjectId = entity.observation_attachment_id;
            Name = entity.attachment_file_name;
            Status = SetStatus(entity.active);
        }
        public string SetStatus(bool? active)
        {
            if (active == true)
            {
                return "Approved";
            }
            else if (active == false)
            {
                return "Denied";
            }
            else
            {
                return "Pending";
            }
        }
    }

    public class RequestModel
    {
        public ICollection<Request> Requests { get; set; }
        public RequestActiveFlags Flags { get; set; }
        public bool Pending { get { return Flags.HasFlag(RequestActiveFlags.Pending); } }
        public bool Approved { get { return Flags.HasFlag(RequestActiveFlags.Approved); } }
        public bool Denied { get { return Flags.HasFlag(RequestActiveFlags.Denied); } }

        public static RequestModel GetRequestModel(int? userId, RequestActiveFlags flags)
        {
            RequestModel model = new RequestModel() { Flags = flags };

            bool allUsers = !userId.HasValue;
            bool pending = flags.HasFlag(RequestActiveFlags.Pending);
            bool approved = flags.HasFlag(RequestActiveFlags.Approved);
            bool Denied = flags.HasFlag(RequestActiveFlags.Denied);

            // var includePredicate = new Func<IRequestEntity, t_user>(e => e.createdby);

            var wherePredicate = new Func<IRequestEntity, bool>(e =>
                (allUsers || e.createdby_userid == userId) 
                //&& ((approved && e.updated_date > DateTime.Now.AddDays(-30)) || (!approved)) 
                //&& ((Denied && e.updated_date > DateTime.Now.AddDays(-30)) || (!Denied)) 
                && ((pending && e.active == null) 
                    || (approved && (e.active ?? false) && e.updated_date > DateTime.Now.AddDays(-30)) 
                    || (Denied && !(e.active ?? true) && e.updated_date > DateTime.Now.AddDays(-30)))
                );

            using (Entity db = new Entity())
            {
                var projects = db.t_project.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_project)e));
                var activities = db.t_activity.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_activity)e));
                var aims = db.t_aim.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_aim)e));
                var indicators = db.t_indicator.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_indicator)e));
                var countries = db.t_country.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_country)e));
                var sites = db.t_site.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_site)e));
                var users = db.t_user.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_user)e));
                var attachments = db.t_observation_attachment.Include(e => e.createdby).Where(wherePredicate).Select(e => new Request((t_observation_attachment)e));
                model.Requests = projects.Concat(activities).Concat(aims).Concat(attachments).Concat(countries).Concat(indicators)
                                         .Concat(sites).Concat(users).ToArray();
            }

            return model;
        }
    }
}