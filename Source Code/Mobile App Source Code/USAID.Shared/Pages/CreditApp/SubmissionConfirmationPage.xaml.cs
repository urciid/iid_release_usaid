using System;
using System.Collections.Generic;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Base;
using USAID.Builders;
using USAID.Common;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
    public class SubmissionConfirmationPageBase : ViewPage<SubmissionConfirmationViewModel>
    {
        
    }

    public partial class SubmissionConfirmationPage : SubmissionConfirmationPageBase
    {
        public SubmissionConfirmationPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false); // should not be able to go back from confirmation screen

        }

        void YesButtonClicked(object sender, System.EventArgs e)
        {
            ViewModel.SendConfirmationEmail();
        }

        void NoButtonClicked(object sender, System.EventArgs e)
        {
            App.TabPageRoot.CurrentPage = App.TabPageRoot.Children[Constants.HomeTab];
        }
    }
}

