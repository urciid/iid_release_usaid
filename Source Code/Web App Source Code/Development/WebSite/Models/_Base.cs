using System;

using IID.BusinessLayer.Identity;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public abstract class Base : BusinessLayer.Models.Base
    {
        protected IidUser CurrentUser
        {
            get
            {
                return Identity.CurrentUser;
            }
        }

        protected int _currentUserId = 0;
        protected int CurrentUserId
        {
            get
            {
                if (_currentUserId == 0)
                    _currentUserId = Identity.CurrentUser.Id;
                return _currentUserId;
            }
        }

        protected string _currentUserFullName;
        protected string CurrentUserFullName
        {
            get
            {
                if (String.IsNullOrEmpty(_currentUserFullName))
                    _currentUserFullName = Identity.CurrentUser.FullName;
                return _currentUserFullName;
            }
        }

        protected string _currentUserEmail;
        protected string CurrentUserEmail
        {
            get
            {
                if (String.IsNullOrEmpty(_currentUserEmail))
                    _currentUserEmail = Identity.CurrentUser.UserName;
                return _currentUserEmail;
            }
        }

        protected byte _currentLanguageId = 0;
        protected byte CurrentLanguageId
        {
            get
            {
                if (_currentLanguageId == 0)
                    _currentLanguageId = IidCulture.CurrentLanguageId;
                return _currentLanguageId;
            }
        }
    }
}