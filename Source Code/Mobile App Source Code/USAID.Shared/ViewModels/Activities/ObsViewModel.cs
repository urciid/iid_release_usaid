using USAID.Interfaces;
using USAID.Base;
using USAID.Utilities;
using System.Windows.Input;
using Xamarin.Forms;
using USAID.Resx;
using System;
using System.Collections.Generic;
using USAID.Models;
using System.Linq;
using USAID.Common;

namespace USAID.ViewModels
{
	public class ObsViewModel : BaseViewModel
	{
		public int ObservationId { get; set;}
		public int ObservationEntryId { get; set; }
		public int IndicatorAgeRangeId { get; set;}
		public string IndicatorGender { get; set;}
		public double Rate { get; set; }
		public string IndicatorType
		{
			//can be age or sex
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}
		public string Title
		{
			//can be age or sex
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string NumeratorName
		{
			get { return GetValue<string>(); }
			set { SetValue(value);  }
		}

		public string NumeratorDefinition
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public int NumeratorValue
		{
			get { return GetValue<int>(); }
			set { SetValue(value); RaisePropertyChanged("IndicatorCountDisplay"); MessagingCenter.Send(this, "NumDemUpdate");}
		}

		public string DenominatorName
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string DenominatorDefinition
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public int DenominatorValue
		{
			get { return GetValue<int>(); }
			set { SetValue(value); RaisePropertyChanged("IndicatorCountDisplay"); MessagingCenter.Send(this, "NumDemUpdate");}
		}

		public string IndicatorName
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string IndicatorDefinition
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public double IndicatorCount
		{
			get { return GetValue<double>(); }
			set { SetValue(value); MessagingCenter.Send(this, "NumDemUpdate");}
		}

		public string IndicatorCountDisplay
		{
			//get display value while using num and dom depending on type
			get 
			{
				var ret = "";
				if (IndicatorType == Constants.IndicatorTypePercentage)
				{
					if (DenominatorValue > 0)
					{
						double perc = ((double)NumeratorValue / (double)DenominatorValue) * 100;
						return string.Format("{0}%", Math.Round(perc,0).ToString());
					}
				}
				if (IndicatorType == Constants.IndicatorTypeAverage)
				{
					if (DenominatorValue > 0)
					{
						double perc = ((double)NumeratorValue / (double)DenominatorValue);
						return string.Format("{0}", Math.Round(perc, 1).ToString());
					}
				}

				if (IndicatorType == Constants.IndicatorTypeRate)
				{
					if (DenominatorValue > 0 && NumeratorValue > 0)
					{
						
						double perc = ((double)NumeratorValue / (double)DenominatorValue) * 1000;
						return string.Format("{0} per {1}", Math.Round(perc, 0).ToString(), 1000);
					}
				}
				if (IndicatorType == Constants.IndicatorTypeCount)
				{
					return string.Format("{0}", Math.Round(IndicatorCount, 1).ToString());
				}

				return ret;
			}

		}
		public bool IndicatorYesNo
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}

		public bool IsIndicatorYesNo
		{
			//get display value while using num and dom depending on type
			get
			{
				var ret = false;
				if (IndicatorType == Constants.IndicatorTypeYesNo)
				{
					return true;
				}

				return ret;
			}
		}

		public bool IsIndicatorCount
		{
			//get display value while using num and dom depending on type
			get
			{
				var ret = false;
				if (IndicatorType == Constants.IndicatorTypeCount)
				{
					return true;
				}

				return ret;
			}
		}

		public bool IsIndicatorNormal
		{
			get
			{
				var ret = false;
				if (IndicatorType == Constants.IndicatorTypePercentage || IndicatorType == Constants.IndicatorTypeAverage || IndicatorType == Constants.IndicatorTypeRate)
				{
					return true;
				}
				return ret;
			}
		}


	}
}
