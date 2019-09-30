using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class LandingPageBase : ViewPage<LandingViewModel>
	{
	}
	public partial class LandingPage : LandingPageBase
    {
		
        public LandingPage()
        {
            InitializeComponent();
        }
    }
}

