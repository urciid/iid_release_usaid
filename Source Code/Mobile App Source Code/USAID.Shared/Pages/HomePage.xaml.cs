using System;
using System.Collections.Generic;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Base;
using USAID.Common;
using USAID.Interfaces;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
    public class HomePageBase : ViewPage<HomeViewModel>
    {
        
    }

    public partial class HomePage : HomePageBase
    {
		
        public HomePage()
        {
			InitializeComponent();
		}

		void EditButton_Clicked(object sender, EventArgs e)
		{
            ShowProfilePage();
		}

		void QuoteButton_Clicked(object sender, EventArgs e)
		{
			App.TabPageRoot.CurrentPage = App.TabPageRoot.Children[Constants.MyQuotesTab];
		}

		void CreditAppButton_Clicked(object sender, EventArgs e)
		{
			App.TabPageRoot.CurrentPage = App.TabPageRoot.Children[Constants.CreditAppTab];
		}

		void HelpButton_Clicked(object sender, EventArgs e)
		{

			Action modalDismissedCallback = () => ViewModel.SetDealerName();
			Navigation.PushModalAsync(new HelpPage());
		
		}

		protected override void OnAppearing()
		{
			//var hasProfileInfo = ViewModel.CheckIfHasProfileInfo();
			//if (!hasProfileInfo)
			//{
   //             ShowProfilePage(true);
			//}
		}

        private void ShowProfilePage(bool missingInformation = false)
        {
            Action modalDismissedCallback = () => ViewModel.SetDealerName();
            Navigation.PushModalAsync(new ProfilePage(modalDismissedCallback, missingInformation));
        }

    }
}

