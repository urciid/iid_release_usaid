using System;
using System.Collections.Generic;
using USAID.Custom;
using USAID.Resx;
using Xamarin.Forms;


namespace USAID.Pages
{

	public class MenuPage : ContentPage
	{
		readonly List<USAIDMenuItem> menuItems = new List<USAIDMenuItem>();

		public ListView Menu { get; set; }

		public MenuPage ()
		{
			menuItems.Add(new USAIDMenuItem { Title = AppResources.HomeMenuText, Padding = "" });
			menuItems.Add(new USAIDMenuItem { Title=AppResources.SitesMenuText, Padding="" });
			//menuItems.Add(new USAIDMenuItem { Title="Help", Padding=""});
			menuItems.Add(new USAIDMenuItem { Title = AppResources.AboutMenuText, Padding = "" });
			//menuItems.Add(new USAIDMenuItem { Title = "Settings", Padding = "" });
			menuItems.Add (new USAIDMenuItem { Title = AppResources.LogOutMenuText, Padding="" });

			BackgroundColor = Color.FromHex("333333");

			var layout = new StackLayout { Spacing = 0, VerticalOptions = LayoutOptions.FillAndExpand };
	
			var stackLayoutSpacer = new StackLayout ();
			stackLayoutSpacer.HeightRequest = 20;
			layout.Children.Add(stackLayoutSpacer);


			Menu = new ListView {
				ItemsSource = menuItems,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent,
			};

			var cell = new DataTemplate(typeof(Custom.MenuCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(TextCell.TextProperty, "Padding");

			cell.SetValue(VisualElement.BackgroundColorProperty, Color.Transparent);
			cell.SetValue (TextCell.TextColorProperty, Color.FromHex("AAAAAA"));


			Menu.ItemTemplate = cell;

			layout.Children.Add(Menu);

			Content = layout;
		}
	}
}


