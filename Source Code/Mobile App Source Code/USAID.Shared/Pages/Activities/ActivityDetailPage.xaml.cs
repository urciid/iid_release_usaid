using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;

namespace USAID.Pages
{
	public class ActivityDetailPageBase : ViewPage<ActivityViewModel>
	{
	}
	public partial class ActivityDetailPage : ActivityDetailPageBase
	{

		public ActivityDetailPage (ActivityModel activity)
		{
			ViewModel.Activity = activity;
			InitializeComponent ();

		}
	}
}

