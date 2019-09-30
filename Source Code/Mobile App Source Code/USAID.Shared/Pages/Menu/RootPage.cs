using System;
using System.Linq;
using USAID.Custom;
using USAID.Resx;
using Xamarin.Forms;



namespace USAID.Pages
{
	public class RootPage : MasterDetailPage
	{
		USAIDMenuItem previousItem;

		public static MenuPage OptionsPage;

		public RootPage()
		{
			Title = "";
			OptionsPage = new MenuPage() { Title = "menu", Icon = "settings" };
			OptionsPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as USAIDMenuItem);


			Master = OptionsPage;

			Detail = App.GetNavigationPage(new LandingPage());

		}

		public void NavigateToPage(Page page)
		{
			if (previousItem != null)
			{
				previousItem.Selected = false;
			}

			Detail = App.GetNavigationPage(page);

			IsPresented = false;
		}

		void NavigateTo(USAIDMenuItem option)
		{
			if (previousItem != null)
				previousItem.Selected = false;

			if (option != null)
			{
				option.Selected = true;
				previousItem = option;
				if (option.Title == AppResources.LogOutMenuText)
				{
					App.ShowLoginScreen(false);
				}
				else {
					var displayPage = PageForOption(option);

					Detail = App.GetNavigationPage(displayPage);

					OptionsPage.Menu.SelectedItem = null;
				}

			}

			IsPresented = false;
		}

		Page PageForOption(USAIDMenuItem option)
		{
			// TODO: Refactor this to the Builder pattern (see ICellFactory).
			if (option.Title == AppResources.HomeMenuText)
				return new LandingPage();
			if (option.Title == AppResources.SitesMenuText)
				return new SitesPage();
			//if (option.Title == "Settings")
			//	return new SettingsPage();
			if (option.Title == "Help")
				return new LandingPage();
			if (option.Title == AppResources.AboutMenuText)
				return new AboutUsPage();
			if (option.Title == AppResources.LogOutMenuText)
				return new LoginPage();
			
			return new LandingPage();
			//throw new NotImplementedException("Unknown menu option: " + option.Title);
		}
	}
}


