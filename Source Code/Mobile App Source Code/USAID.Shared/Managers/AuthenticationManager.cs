using System;
using System.Threading.Tasks;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Interfaces;
using USAID.Managers;
using Xamarin.Forms;

namespace USAID.Managers
{
	public class AuthenticationManager : IAuthenticationManager
	{
		public string AuthToken {get; set;}

        public async Task<bool> Authenticate()
        {
            var authService = AppContainer.Container.Resolve<IAuthenticationService>();
            try
            {
                // Initial login sets the user info via the UserInfoService, this is where we will get it
                var userInfo = AppContainer.Container.Resolve<IUserInfoService>().GetSavedInfo();

				var authResult = await authService.Authenticate(userInfo.Email, userInfo.Password);

				if (authResult != null && authResult.Access_Token != null)
                {
					AuthToken = authResult.Access_Token;
					AppContainer.Container.Resolve<IUserInfoService>().SaveUserInfo(new Models.UserInfoModel
					{
						Email = userInfo.Email,
						Password = userInfo.Password,
						Token = AuthToken,
						LastDownload = userInfo.LastDownload
					});
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetAuthToken()
        {
			var userInfo = AppContainer.Container.Resolve<IUserInfoService>().GetSavedInfo();
			var token = AuthToken == null ? userInfo.Token : AuthToken;
			return token;
        }
    }
}

