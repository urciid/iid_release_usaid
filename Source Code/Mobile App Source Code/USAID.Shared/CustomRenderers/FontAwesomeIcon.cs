using System;

using Xamarin.Forms;

namespace USAID.CustomRenderers
{
	public class FontAwesomeIcon : Label
	{
		//Must match the exact "Name" of the font which you can get by double clicking the TTF in Windows
		public const string Typeface = "FontAwesome";  

		public FontAwesomeIcon (string fontAwesomeIcon = null)
		{
			FontFamily = Typeface;    //iOS is happy with this, Android needs a renderer to add ".ttf"
			Text = fontAwesomeIcon;
		}

		public FontAwesomeIcon ()
		{
			FontFamily = Typeface;    //iOS is happy with this, Android needs a renderer to add ".ttf"
		}

		public static readonly BindableProperty CommandParameterProperty = 
			BindableProperty.Create<FontAwesomeIcon, string> (
				p => p.CommandParameter, String.Empty);

		public string CommandParameter 
		{
			get { return (string)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		/// <summary>
		/// Get more icons from http://fortawesome.github.io/Font-Awesome/cheatsheet/
		/// Tip: Just copy and past the icon picture here to get the icon
		/// </summary>
		public static class Icon
		{
			public static readonly string Gear = "";
			public static readonly string Bars = "";
			public static readonly string Start = "";
			public static readonly string Stop = "";
			public static readonly string Pause = "";
			public static readonly string Power = "";
			public static readonly string Reboot = "";
			public static readonly string Reset = "";
			public static readonly string Maintenance = "";
			public static readonly string DownArrow = "";
			public static readonly string UpArrow = "";
			public static readonly string Dashboard = "";
			public static readonly string BarGraph = "";
			public static readonly string LogOff = "";
			public static readonly string DataCenter = "";
			public static readonly string RightArrow = "";
			public static readonly string LeftArrow = "";
			public static readonly string Folder = "";
		}
	}
}

