using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using USAID.CustomRenderers;
using USAID.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(NoHelperEntry), typeof(NoHelperEntryRenderer))]
namespace USAID.iOS.CustomRenderers
{
    // Entry without autocorrection in iOS
    // Taken from https://developer.xamarin.com/recipes/cross-platform/xamarin-forms/ios/prevent-keyboard-suggestions/
    public class NoHelperEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.SpellCheckingType = UITextSpellCheckingType.No;             // No Spellchecking
                Control.AutocorrectionType = UITextAutocorrectionType.No;           // No Autocorrection
                Control.AutocapitalizationType = UITextAutocapitalizationType.None; // No Autocapitalization
            }
        }
    }
}

