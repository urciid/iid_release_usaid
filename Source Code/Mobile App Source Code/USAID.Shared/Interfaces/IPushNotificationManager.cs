using System;
namespace WMP.Shared.Interfaces
{
	public interface IPushNotificationManager
	{
		void RegisterApplication();
		void UnregisterApplication();
		string LoadDeviceToken();
		void SaveDeviceToken(string token);
	}
}








