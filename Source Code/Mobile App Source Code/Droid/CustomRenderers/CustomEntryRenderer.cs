using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics;
using USAID.Droid.CustomRenderers;
using USAID.CustomRenderers;
using Android.Views;
using Android.Util;

[assembly: ExportRenderer (typeof (CustomEntry), typeof (CustomEntryRenderer))]
namespace USAID.Droid.CustomRenderers
{
	public class CustomEntryRenderer : EntryRenderer
	{
		// Override the OnElementChanged method so we can tweak this renderer post-initial setup
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

	
			if (Control != null)
			{
				// do whatever you want to the textField here!
				var nativeEditText = (global::Android.Widget.EditText)Control;
				// do whatever you want to the EditText here!

				//nativeEditText.SetBackgroundColor (global::Android.Graphics.Color.DarkGray);
				nativeEditText.SetBackgroundColor(global::Android.Graphics.Color.White);
				nativeEditText.SetTextColor(global::Android.Graphics.Color.Black);
				nativeEditText.TextAlignment = Android.Views.TextAlignment.Center;
				nativeEditText.Gravity = GravityFlags.CenterHorizontal;
				//Control.SetBackgroundColor(global::Android.Graphics.Color.White);
				var shape = new ShapeDrawable(new RectShape());
				shape.Paint.Color = global::Android.Graphics.Color.Black;
				shape.Paint.StrokeWidth = 2;
				shape.Paint.SetStyle(Paint.Style.Stroke);

				nativeEditText.SetBackgroundDrawable(shape);

			}

			//if (Control != null) {
			//	// do whatever you want to the textField here!
			//	var nativeEditText = (global::Android.Widget.EditText)Control;
			//	// do whatever you want to the EditText here!

			//	//nativeEditText.SetBackgroundColor (global::Android.Graphics.Color.DarkGray);
			//	nativeEditText.SetBackgroundColor(global::Android.Graphics.Color.White);
			//	nativeEditText.SetTextColor(global::Android.Graphics.Color.Black);
			//	nativeEditText.TextSize = 14f;
			//	//Control.SetBackgroundColor(global::Android.Graphics.Color.White);
			//	//var shape = new ShapeDrawable(new RectShape());
			//	//shape.Paint.Color = global::Android.Graphics.Color.Black;
			//	//shape.Paint.StrokeWidth = 2;
			//	//shape.Paint.SetStyle(Paint.Style.Stroke);

			//	//nativeEditText.SetBackgroundDrawable(shape);



			//}
		}
	}
}

