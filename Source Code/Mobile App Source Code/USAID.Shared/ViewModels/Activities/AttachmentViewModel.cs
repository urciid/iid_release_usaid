using USAID.Interfaces;
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
using System.IO;

namespace USAID.ViewModels
{
	public class AttachmentViewModel : BaseViewModel
	{
		private string message = "AttachmentScreen";
		private readonly IUserInfoService _userInfoService;
		private readonly IHUDProvider _hudProvider;
		public INavigation _Navigation { get; set; }
		public ObservationAttachment OriginalAttachment { get; set; }

		public ObservationAttachment Attachment
		{
			get { return GetValue<ObservationAttachment>(); }
			set { SetValue(value); }
		}

		public ImageSource ImageSelected
		{
			get
			{
				var stream = new MemoryStream(Attachment.Bytes);
				return ImageSource.FromStream(() => stream);
			}
		}

		public ICommand CancelCommand { get { return new SimpleCommand(Cancel); } }

		private async void Cancel()
		{
			MessagingCenter.Send(Attachment, "AttachmentScreenCancel");

		}



		public AttachmentViewModel(IGALogger logger, IUserInfoService userInfoService,
			IHUDProvider hudProvider)
		{
			_userInfoService = userInfoService;
			_hudProvider = hudProvider;
		}
	}
}
