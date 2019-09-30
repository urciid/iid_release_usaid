
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using USAID.Base;

namespace USAID.ViewModels
{
	public abstract class BaseViewModel : INotifyPropertyChanged, IViewModel
	{
		private readonly Dictionary<string, object> PropertyValues = new Dictionary<string, object>();

		public event PropertyChangedEventHandler PropertyChanged = delegate { };


		public string PageTitle
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public bool IsBusy
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}

		protected T GetValue<T>([CallerMemberName] string propertyName = null)
		{
			return GetValue(propertyName, default(T));
		}

		protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
		{
			
			if (string.IsNullOrEmpty(propertyName)) return;

			var shouldNotify = !PropertyValues.ContainsKey(propertyName) || !object.Equals(value, PropertyValues[propertyName]);

			PropertyValues[propertyName] = value;

			if (shouldNotify)
			{
				RaisePropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// Gets the value of the property specified by propertyName. If no
		/// value is present, defaultValue is returned.
		/// </summary>
		/// <param name="propertyName">The name of the property for which you're
		/// trying to get the value of.</param>
		/// <param name="propertyName">The name of the property (note this is case sensitive)
		/// for which you're trying to get the value of</param>
		private T GetValue<T>(string propertyName, T defaultValue)
		{
			if (PropertyValues.ContainsKey(propertyName))
				return (T)PropertyValues[propertyName];

			return defaultValue;
		}

		public void RaisePropertyChanged(string propName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

	}
}