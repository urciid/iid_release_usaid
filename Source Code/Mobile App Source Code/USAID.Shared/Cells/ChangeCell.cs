using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace USAID.Cells
{
	public class ChangeCell : ViewCell
	{
		public ChangeCell(string change)
		{
			var button = new Button();
			//button.BackgroundColor = Color.Red;
			//button.SetBinding(Button.TextProperty, "Title");
			//button.Clicked += (object sender, EventArgs e) =>
			//{
			//	//App.CurrentApp.MainPage.DisplayAlert("button", "Clicked.", "OK");
			//	Debug.WriteLine("here");
			//};
			var label = new Label();
			label.Text = change;
			label.BackgroundColor = Color.Transparent;
			label.VerticalTextAlignment = TextAlignment.Center;

			var s = new StackLayout();
			s.Children.Add(label);
			this.View = s;
		}
	}
}
