using System;
using System.Collections.Generic;
using USAID.Base;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace USAID.Pages
{
	public class CommentPageBase : ViewPage<CommentViewModel>
	{
	}
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CommentPage : CommentPageBase
	{


		private ObservationComment _comment;
		public CommentPage(ObservationComment comment = null)
		{
			InitializeComponent();
			buttonLayout.BackgroundColor = Color.FromHex("#034468");
			NavigationPage.SetBackButtonTitle(this, "Cancel");
			if (comment == null)
			{
				_comment = new ObservationComment { Created_Date = DateTime.Now };
			}
			else {
				_comment = comment;
			}
			ViewModel.Comment = _comment;
			ViewModel.OriginalComment = _comment;
			if (Device.OS == TargetPlatform.Android)
			{
				BoxView.IsVisible = false;
			}
		}

		async void CancelClicked(object sender, System.EventArgs e)
		{
			var answer = await DisplayAlert("Unsaved Changes", "Are you sure you want to leave without saving?", "Yes", "No");
			if (answer)
			{
				await Navigation.PopAsync(true);
			}
		}
	}
}
