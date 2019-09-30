using Xamarin.Forms;
using USAID.iOS.Providers;
using Foundation;
using USAID.Interfaces;
using UIKit;

[assembly: Dependency(typeof(PhoneCallService))]
namespace USAID.iOS.Providers
{
	public class PhoneCallService : IPhoneCallService
	{
		/// <summary>
		/// Used for registration with dependency service
		/// </summary>
		public static void Init ()
		{
		}

		/// <summary>
		/// Makes the call.
		/// </summary>
		/// <param name="phoneNumber">Phone number.</param>
		public void MakeCall (string phoneNumber)
		{
			NSUrl url = new NSUrl (string.Format (@"telprompt://{0}", phoneNumber));
			UIApplication.SharedApplication.OpenUrl (url);
		}
	}
}



