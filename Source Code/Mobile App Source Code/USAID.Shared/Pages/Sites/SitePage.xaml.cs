using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class SitePageBase : ViewPage<SiteViewModel>
	{
	}
	public partial class SitePage : SitePageBase
	{

		public SitePage(Site site)
		{
			ViewModel.Site = site;
			InitializeComponent();

		}
	}
}

