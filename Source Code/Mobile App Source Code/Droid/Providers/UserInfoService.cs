using System;

using Android.Content;
using Android.Preferences;
using USAID.Common;
using USAID.Interfaces;
using USAID.Models;
using Xamarin.Forms;
using GA.Droid.Providers;
using USAID;
using Java.IO;

[assembly: Dependency(typeof(UserInfoService))]
namespace GA.Droid.Providers
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
		public void ClearUserInfo()
		{
			var userInfo = new UserInfoModel ();
			userInfo.Email = String.Empty;
			userInfo.Password = String.Empty;
			userInfo.LastDownload = string.Empty;
			SaveUserInfo(userInfo);
		}

		/// <summary>
		/// Sets the user credentials.
		/// </summary>
		/// <param name="userCredentials">User credentials.</param>
		public void SaveUserInfo(UserInfoModel userInfo)
		{
			ISharedPreferences preferenceManager = PreferenceManager.GetDefaultSharedPreferences (Android.App.Application.Context); 
			ISharedPreferencesEditor editor = preferenceManager.Edit ();
			editor.PutString (Constants.EmailKey, userInfo.Email);
			editor.PutString (Constants.PasswordKey, userInfo.Password);
			editor.PutString(Constants.TokenKey, userInfo.Token);
			editor.PutString(Constants.LastDownloadKey, userInfo.LastDownload);
			editor.Apply ();
		}

		/// <summary>
		/// Gets the user info.
		/// </summary>
		/// <returns>The user info model.</returns>
		public UserInfoModel GetSavedInfo()
		{
			ISharedPreferences preferenceManager = PreferenceManager.GetDefaultSharedPreferences (Android.App.Application.Context); 
			var email = preferenceManager.GetString (Constants.EmailKey, String.Empty);
			var password = preferenceManager.GetString (Constants.PasswordKey, String.Empty);
			var token = preferenceManager.GetString(Constants.TokenKey, string.Empty);
			var lastDownload = preferenceManager.GetString(Constants.LastDownloadKey, string.Empty);

            return new UserInfoModel()
            {
				Email = email,
				Password = password,
				Token = token,
				LastDownload = lastDownload
            };
		}

        public void SavePhotoFilePath(string photoFilePath)
        {
            ISharedPreferences preferenceManager = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            ISharedPreferencesEditor editor = preferenceManager.Edit();
            editor.PutString(Constants.PhotoFilePathKey, photoFilePath);
            editor.Apply();
        }

        public string GetPhotoFilePath()
        {
            ISharedPreferences preferenceManager = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
            var photoFilePath = preferenceManager.GetString(Constants.PhotoFilePathKey, String.Empty);
            return photoFilePath;
        }

        public void DeleteAllPhotos()
        {
            var photoDirPath = GetPhotoFilePath();
            if (string.IsNullOrWhiteSpace(photoDirPath))
            {
                return;
            }

            File dir = new File(photoDirPath);
            if (dir.IsDirectory)
            {
                foreach (var photo in dir.List())
                {
                    new File(dir, photo).Delete();
                }
            }
        }
	}
}



