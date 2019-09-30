using System;

namespace USAID.Interfaces
{
	public interface IDeviceSettings
	{
		string CarrierName { get; set; }
		string PhoneModel { get; set; }
		string PhonePlatform { get; set; }
		string PhoneOS { get; set; }
		string AppVersion { get; set; }
		bool HasSDCard { get; set; }
		bool HasCarrierDataNetwork { get; set; }
		int GetCurrentUTCOffset();
		void LoadInfo();
	}
}


