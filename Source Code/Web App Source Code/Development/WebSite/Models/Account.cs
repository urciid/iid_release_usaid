using System.ComponentModel.DataAnnotations;

using IID.WebSite.Helpers;

using error = IID.BusinessLayer.Globalization.Error.Resource;
using login = IID.BusinessLayer.Globalization.Login.Resource;

namespace IID.WebSite.Models
{
    public class ForgotViewModel
    {
        [Display(Name = "EmailAddress", ResourceType = typeof(login))]
        [EmailAddress(ErrorMessageResourceType = typeof(login), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string EmailAddress { get; set; }
    }

    public class LoginViewModel
    {
        [Display(Name = "EmailAddress", ResourceType = typeof(login))]
        [EmailAddress(ErrorMessageResourceType = typeof(login), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string EmailAddress { get; set; }

        [Display(Name = "Password", ResourceType = typeof(login))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Display(Name = "EmailAddress", ResourceType = typeof(login))]
        [EmailAddress(ErrorMessageResourceType = typeof(login), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string EmailAddress { get; set; }

        [Display(Name = "Password", ResourceType = typeof(login))]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(login))]
        [Compare("Password", ErrorMessageResourceType = typeof(login), ErrorMessageResourceName = "PasswordsDontMatch")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Display(Name = "EmailAddress", ResourceType = typeof(login))]
        [EmailAddress(ErrorMessageResourceType = typeof(login), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(error), ErrorMessageResourceName = "Required")]
        public string EmailAddress { get; set; }
    }
}
