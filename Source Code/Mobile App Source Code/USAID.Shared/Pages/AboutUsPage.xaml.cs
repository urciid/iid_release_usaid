using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class AboutUsPageBase : ViewPage<AboutUsViewModel>
	{
	}
	public partial class AboutUsPage : AboutUsPageBase
	{

		public AboutUsPage()
		{
			InitializeComponent();
		}
	}
}

