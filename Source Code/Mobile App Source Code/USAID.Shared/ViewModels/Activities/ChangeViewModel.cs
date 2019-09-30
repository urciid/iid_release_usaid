﻿using USAID.Interfaces;
using USAID.Base;
using USAID.Utilities;
using System.Windows.Input;
using Xamarin.Forms;
using USAID.Resx;
using System;
using System.Collections.Generic;
using USAID.Models;
using System.Linq;
using USAID.Common;
using System.Threading.Tasks;
using USAID.Pages;
using System.Collections.ObjectModel;

namespace USAID.ViewModels
{
	public class ChangeViewModel : BaseViewModel
	{
		private string message = "ChangeScreen";
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		public INavigation _Navigation { get; set; }
		public ObservationChange OriginalChange { get; set; }

		public ObservationChange Change
		{
			get { return GetValue<ObservationChange>(); }
			set { SetValue(value); }
		}


		public ICommand SaveCommand { get { return new SimpleCommand(Save); } }

		private void Save()
		{
			//TODO:language
			_hudProvider.DisplayProgress("Saving");

			//MessagingCenter.Send<ChangePage, Change>(this, "ChangeComplete", _change);
			MessagingCenter.Send(Change, message);
			_hudProvider.Dismiss();
		}

		public ICommand CancelCommand { get { return new SimpleCommand(Cancel); } }

		private async void Cancel()
		{
			//TODO:language
			var answer = await App.CurrentApp.MainPage.DisplayAlert("Unsaved Changes", "Are you sure you want to leave without saving?", "Yes", "No");
			if (answer)
			{
				Change = OriginalChange;
				MessagingCenter.Send(Change, "ChangeScreenCancel");
			}
		}



		public ChangeViewModel(IGALogger logger, IUserInfoService userInfoService,
			IHUDProvider hudProvider)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
		}
	}
}
