using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using IID.BusinessLayer.Domain;
using IID.BusinessLayer.Identity;
using IID.WebSite.Helpers;
using IID.WebSite.Models;
using System.Threading.Tasks;

namespace IID.WebSite.Controllers
{
    [NoCache]
    public class UserController : BaseController
    {
        [Authorize(Roles = IidRole.SystemAdministratorRole)]
        public ActionResult Manage()
        {

            using (Entity db = new Entity())
            {

                ICollection<v_user> users = db.v_user.OrderBy(e => e.email).ToArray();

                if (users == null)
                    throw new ArgumentException("Error getting users.");

                return View(ManageUsersViewModel.FromDatabase(users));
            }
        }


        public ActionResult View(int id)
        {
            return View(new User(id));
        }

        public ActionResult Add()
        {
            return View(new User());
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Add(User model)
        {
            if (ModelState.IsValid)
            {
                bool isAdmin = CurrentUser.IsInRole(Role.SystemAdministrator);

                using (Entity db = new Entity())
                {
                    string emailLowerCase = model.Email.ToLower();
                    var existingUser = db.t_user.FirstOrDefault(u => u.email.ToLower() == emailLowerCase);
                    if (existingUser == null)
                    {
                        // Set and save user fields.
                        t_user user = new t_user();
                        db.t_user.Add(user);
                        SetCoreFields(ref user, model);
                        user.consecutive_failed_login_attempts = 0;
                        user.security_stamp = new Guid().ToString();
                        user.createdby_userid = CurrentUser.Id;
                        user.created_date = DateTime.Now;
                        db.SaveChanges();

                        // Save the user role.
                        SetUserRole(user.user_id, model.UserRoleFieldIds);

                        // If an admin saved the user, it is approved, so send the welcome email.
                        if (isAdmin && user.active == true)
                            Identity.GetUserManager().InviteUser(user.email);

                        return RedirectToAction("View", new { id = user.user_id });
                    }
                    else
                    {
                        ModelState.AddModelError("", $"A user with the email {model.Email} already exists.");
                    }
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            return View(new User(id));
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model)
        {
            if (ModelState.IsValid)
            {
                using (Entity db = new Entity())
                {
                    if (!model.UserId.HasValue)
                        throw new ArgumentException("userId not provided");

                    t_user user = db.t_user.Find(model.UserId);
                    if (user == null)
                        throw new ArgumentException("Invalid userId: " + model.UserId.ToString());

                    // If the user was unapproved but is being approved now, send the welcome email.
                    bool approvingUser =
                        CurrentUser.IsInRole(Role.SystemAdministrator) &&
                        !user.active.HasValue && model.Active.HasValue;

                    // Set and save user fields.
                    SetCoreFields(ref user, model);
                    user.updatedby_userid = CurrentUser.Id;
                    user.updated_date = DateTime.Now;
                    db.SaveChanges();

                    // Save user role.
                    SetUserRole(user.user_id, model.UserRoleFieldIds);

                    // Send the welcome email if necessary.
                    if (approvingUser)
                        Identity.GetUserManager().InviteUser(user.email);

                    return RedirectToAction("View", new { id = user.user_id });
                }
            }
            else
            {
                return View(model);
            }
        }

        private void SetCoreFields(ref t_user user, User model)
        {
            user.email = model.Email;
            user.first_name = model.FirstName;
            user.last_name = model.LastName;
            user.title = model.Title;
            user.phone = model.Phone;
            user.site_id = model.SiteId;
            user.active = model.Active;
            user.user_status_id = 2;  //default to 2 this column is not used at moment
        }

        private void SetUserRole(int userId, ICollection<string> roleFieldIds)
        {
            using (Entity db = new Entity())
            {
                // Remove pre-existing role(s).
                db.t_user_role.RemoveRange(db.t_user_role.Where(e => e.user_id == userId));

                if (roleFieldIds != null && roleFieldIds.Count > 0)
                {
                    foreach (string roleFieldId in roleFieldIds)
                    {
                        if (!String.IsNullOrEmpty(roleFieldId))
                        {
                            // Assign new role.
                            db.t_user_role.Add(new t_user_role()
                            {
                                user_id = userId,
                                user_role_fieldid = roleFieldId,
                                createdby_userid = CurrentUser.Id,
                                created_date = DateTime.Now
                            });
                        }
                    }
                }

                db.SaveChanges();
            }
        }
    }
}