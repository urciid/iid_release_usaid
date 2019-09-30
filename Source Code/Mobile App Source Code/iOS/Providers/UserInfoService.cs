using Xamarin.Forms;
using USAID.iOS.Providers;
using Foundation;
using USAID.Common;
using USAID.Interfaces;
using USAID.Models;
using System;

[assembly: Dependency(typeof(UserInfoService))]
namespace USAID.iOS.Providers
{
	public class UserInfoService : IUserInfoService
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init ()
		{
		}

		/// <summary>
		/// Clears the user credentials.
		/// </summary>
		public void ClearUserInfo(){
			NSUserDefaults.StandardUserDefaults.RemoveObject(Constants.EmailKey);
			NSUserDefaults.StandardUserDefaults.RemoveObject(Constants.PasswordKey);
			NSUserDefaults.StandardUserDefaults.RemoveObject(Constants.TokenKey);
			NSUserDefaults.StandardUserDefaults.RemoveObject(Constants.LastDownloadKey);

			NSUserDefaults.StandardUserDefaults.Synchronize();
		}

		public void SaveUserInfo(UserInfoModel infoModel)
		{
			NSUserDefaults.StandardUserDefaults.SetString(infoModel.Email, Constants.EmailKey);
			NSUserDefaults.StandardUserDefaults.SetString(infoModel.Password, Constants.PasswordKey);
			NSUserDefaults.StandardUserDefaults.SetString(infoModel.Token, Constants.TokenKey);
			NSUserDefaults.StandardUserDefaults.SetString(infoModel.LastDownload, Constants.LastDownloadKey);
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}

		public UserInfoModel GetSavedInfo()
		{
			var emailKey = NSUserDefaults.StandardUserDefaults.StringForKey(Constants.EmailKey);
			var passKey = NSUserDefaults.StandardUserDefaults.StringForKey(Constants.PasswordKey);
			var token = NSUserDefaults.StandardUserDefaults.StringForKey(Constants.TokenKey);
			var lastDownload = NSUserDefaults.StandardUserDefaults.StringForKey(Constants.LastDownloadKey);


			return new UserInfoModel()
			{
				Email = emailKey,
				Password = passKey,
				Token = token,
				LastDownload = lastDownload
			};
		}

        public void SavePhotoFilePath(string photoFilePath)
        {
            NSUserDefaults.StandardUserDefaults.SetString(photoFilePath, Constants.PhotoFilePathKey);
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        public string GetPhotoFilePath()
        {
            var photoFilePath = NSUserDefaults.StandardUserDefaults.StringForKey(Constants.PhotoFilePathKey);
            return photoFilePath;
        }

        public void DeleteAllPhotos()
        {
            var photoDirPath = GetPhotoFilePath();
            if (string.IsNullOrWhiteSpace(photoDirPath))
            {
                return;
            }

            NSFileManager fm = new NSFileManager();
            NSError error; //TODO: error handling?
            var fileNames = fm.GetDirectoryContent(photoDirPath, out error);
            foreach (var fileName in fileNames)
            {
                fm.Remove(photoDirPath + "/" + fileName, out error);
            }
        }
	}
}



