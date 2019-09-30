using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace USAID.Custom
{
	public class MenuCell : ViewCell
	{
		public MenuCell()
		{
			StackLayout sLayout = new StackLayout ();
			sLayout.BackgroundColor = Color.Transparent;
			sLayout.VerticalOptions = LayoutOptions.Center;
			sLayout.Orientation = StackOrientation.Horizontal;

			var spacingLabel = new Label ();
			spacingLabel.SetBinding(Label.TextProperty, "Padding");

			var label = new Label ();
			label.SetBinding(Label.TextProperty, "Title");
			label.TextColor = Color.FromHex ("AAAAAA");
			if (label.Text == "Company Search") {
				sLayout.Padding = new Thickness (50, 10, 0, 0);
			} else {
				sLayout.Padding = new Thickness (20, 10, 0, 0);
			}
			sLayout.Children.Add (spacingLabel);
			sLayout.Children.Add (label);
			this.View = sLayout;

		}
	}
}
