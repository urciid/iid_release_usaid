using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using USAID.CustomRenderers;
using USAID.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(CustomerPickerRenderer))]
namespace USAID.iOS.CustomRenderers
{
	// Entry without autocorrection in iOS
	// Taken from https://developer.xamarin.com/recipes/cross-platform/xamarin-forms/ios/prevent-keyboard-suggestions/
	public class CustomerPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);
			if (Control != null)
			{
				Control.BorderStyle = UITextBorderStyle.None;
			}
		}
	}
}

