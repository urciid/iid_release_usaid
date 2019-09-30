using System;
using USAID.Models;

namespace USAID.Interfaces
{
	public interface IUserInfoService
	{
		void SaveUserInfo(UserInfoModel info);
        UserInfoModel GetSavedInfo();
        void SavePhotoFilePath(string photoFilePath);
        string GetPhotoFilePath();
        void DeleteAllPhotos();
		void ClearUserInfo ();
	}
}

