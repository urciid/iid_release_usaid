using System;
using Xamarin.Forms;

namespace USAID.Common
{
	public class Utils
	{
		/// <summary>
		/// Gets the navigation page.
		/// </summary>
		/// <returns>The navigation page.</returns>
		/// <param name="page">Page.</param>
		public static NavigationPage GetNavigationPage(Page page)
		{
			var navigationPage = new NavigationPage (page);

			navigationPage.BarBackgroundColor = Color.FromHex("#034468");
			navigationPage.BarTextColor = Color.White;

			return navigationPage;
		}
	}
}

