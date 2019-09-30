using System;
using Xamarin.Forms;
using System.Collections.Generic;
using WMP.Shared.ViewModels;
using System.Threading.Tasks;


namespace WMP.Shared.Pages
{


	public class RootPageold : MasterDetailPage
	{
		Dictionary<MenuType, NavigationPage> Pages { get; set; }

		public RootPageold()
		{
			Pages = new Dictionary<MenuType, NavigationPage>();
			Master = new MenuPage(this);
			BindingContext = new BaseViewModel(Navigation)
			{
				Title = "Xamarin CRM",
				Icon = "slideout.png"
			};
			//setup home page
			NavigateAsync(MenuType.Item1);
		}

		void SetDetailIfNull(Page page)
		{
			if (Detail == null && page != null)
				Detail = page;
		}

		public async Task NavigateAsync(MenuType id)
		{
			Page newPage;
			if (!Pages.ContainsKey(id))
			{
				switch (id)
				{
				case MenuType.Item1:
//					var page = new NavigationPage(new LandingPage
//						{ 
//							Title = "Landing Page", 
//							Icon = new FileImageSource { File = "sales.png" }
//						});
//					SetDetailIfNull(page);
//					Pages.Add(id, page);
					break;
				}
			}

			newPage = Pages[id];
			if (newPage == null)
				return;

			//pop to root for Windows Phone
			if (Detail != null && Device.OS == TargetPlatform.WinPhone)
			{
				await Detail.Navigation.PopToRootAsync();
			}

			Detail = newPage;

			if (Device.Idiom != TargetIdiom.Tablet)
				IsPresented = false;
		}
	}


	public enum MenuType
	{
		Item1,
		Item2,
		Item3,
		Item4
	}

	public class HomeMenuItem
	{
		public HomeMenuItem()
		{
			MenuType = MenuType.Item1;
		}

		public string Icon { get; set; }

		public MenuType MenuType { get; set; }

		public string Title { get; set; }

		public string Details { get; set; }

		public int Id { get; set; }
	}


}



