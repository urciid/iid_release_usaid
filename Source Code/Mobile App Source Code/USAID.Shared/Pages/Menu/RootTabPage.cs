using System;
using Xamarin.Forms;
using USAID.ViewModels;
using USAID.Pages;
using USAID.Common;
using USAID.ApplicationObjects;
using Autofac;

namespace USAID.Pages
{
	public class RootTabPage : TabbedPage
	{
        private const int numTabs = 3;

        private string lastPageTitle;
        
		public RootTabPage()
		{
			this.ToolbarItems.Add(new ToolbarItem
			{
				Text = "Help",
				Icon = new FileImageSource { File = "gaHelpIconWhite.png"  },
				Command = new Command(() => this.Navigation.PushModalAsync(new HelpPage()))
			});
			this.ToolbarItems.Add(new ToolbarItem
			{
				Text="Profile",
				Icon = new FileImageSource { File = "gaProfileIconWhite.png" },
				Command = new Command(() => this.Navigation.PushModalAsync(new ProfilePage()))
			});


			Children.Add(new NavigationPage(new HomePage { Title = "Home" })
			{ 
				BarTextColor = Color.White,
				BarBackgroundColor = Color.FromHex("#034468"),
                //Title = Constants.HomeTabTitle, 
				Icon = new FileImageSource { File = "gaHomeIcon@2x.png" }
            });
            Children.Add(new NavigationPage(new QuotesPage { Title = "My Quotes" })
			{
				BarTextColor = Color.White,
				BarBackgroundColor = Color.FromHex("#034468"),
                Title = Constants.MyQuotesTabTitle,
				Icon = new FileImageSource { File = "gaQuoteIcon@2x.png" }
            });
            Children.Add(new NavigationPage(new CustomerInfoPage())
			{
				BarTextColor = Color.White,
				BarBackgroundColor = Color.FromHex("#034468"),
                Title = Constants.CreditAppTabTitle,
				Icon = new FileImageSource { File = "gaCreditAppIcon@2x.png" }
            });
		}

		protected override void OnCurrentPageChanged()
		{
			base.OnCurrentPageChanged();
			this.Title = this.CurrentPage.Title;

            if (Children.Count >= numTabs) // because this is called when RootTabPage is created
            {
                CurrentPage.Navigation.PopToRootAsync();

                switch (lastPageTitle) //clean up any state before going back to this tab
                {
                    case Constants.HomeTabTitle:
                        break;
                    case Constants.MyQuotesTabTitle:
                        break;
                    case Constants.CreditAppTabTitle: // user just left the Credit App tab
                        var creditAppBuilder = AppContainer.Container.Resolve<ICreditAppBuilder>();
                        creditAppBuilder.CreateCreditApp(); //wipe away current credit app
                        break;
                }
            }

            lastPageTitle = CurrentPage.Title;
		}

	
	}
}

