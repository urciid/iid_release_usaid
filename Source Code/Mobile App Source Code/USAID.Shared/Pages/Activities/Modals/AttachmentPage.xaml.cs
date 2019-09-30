using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace USAID.Pages
{
	public class AttachmentPageBase : ViewPage<AttachmentViewModel>
	{
	}
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AttachmentPage : AttachmentPageBase
	{


		private ObservationAttachment _attachment;
		public AttachmentPage(ObservationAttachment attachment = null)
		{
			InitializeComponent();
			buttonLayout.BackgroundColor = Color.FromHex("#034468");
			NavigationPage.SetBackButtonTitle(this, "Cancel");
			if (attachment == null)
			{
				_attachment = new ObservationAttachment { Bytes = null };
			}
			else
			{
				_attachment = attachment;
			}
			ViewModel.Attachment = _attachment;
		}

		async void CancelClicked(object sender, System.EventArgs e)
		{
			await Navigation.PopAsync(true);
		}
	}
}
