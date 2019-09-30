using System;
using System.Collections.Generic;
using System.Linq;
using USAID.Base;
using USAID.Common;
using USAID.Models;
using USAID.ViewModels;
using Xamarin.Forms;
using Xuni.Forms.FlexChart;

namespace USAID.Pages
{
	public class ChartPageBase : ViewPage<ChartViewModel>
	{
	}
	public partial class ChartPage : ChartPageBase
	{
		public ChartPage(IndicatorVM indicator, int siteId)
		{
			InitializeComponent();
			ViewModel.Indicator = indicator;
			ViewModel.SiteId = siteId;
			var list = ViewModel.CreateEntityList();
		


			if (ViewModel.Indicator.IndicatorType == Constants.IndicatorTypePercentage)
			{
				flexChart.AxisY.Max = 100;
				flexChart.AxisY.Min = 0;
				flexChart.AxisY.MajorUnit = 10;
			}
			else
			//if (ViewModel.Indicator.IndicatorType == Constants.IndicatorTypeAverage)
			{
				if (list.Count() > 0)
				{
					var max = list.ToList().Max(m => m.CountDisplay);
					var min = list.ToList().Min(m => m.CountDisplay);
					var diff = max - min;


					if (diff < 1)
					{
						flexChart.AxisY.MajorUnit = .1;
					}
					else {
						var y = Math.Round(diff / 10, 0);
						if (y > 50 && y < 150)
							y = 100;
						flexChart.AxisY.MajorUnit = y;
					}
					var items = list.ToList().Count();

					flexChart.AxisY.Max = max;
					flexChart.AxisY.Min = 0; //min - flexChart.AxisY.MajorUnit;
				}


			}
			this.flexChart.AxisY.LabelLoading += AxisYLabeLoading;
			this.flexChart.AxisX.LabelLoading += AxisXLabeLoading;
			flexChart.ItemsSource = list;

			flexChart.AxisX.MinorTickWidth = 1;
			flexChart.AxisX.MajorTickWidth = 2;

			flexChart.Tooltip.Threshold = 50;
		}
		void AxisXLabeLoading(object sender, LabelLoadingEventArgs e)
		{
			Label label = new Label();
			label.VerticalTextAlignment = TextAlignment.Center;
			label.HorizontalTextAlignment = TextAlignment.End;
			label.Text = string.Format("{0}", e.Text);
			Device.OnPlatform(iOS: () => label.FontSize = 9);
			Device.OnPlatform(Android: () => label.FontSize = 9);

			e.Label = label;
		}
		void AxisYLabeLoading(object sender, LabelLoadingEventArgs e)
		{
			Label label = new Label();
			label.VerticalTextAlignment = TextAlignment.Center;
			label.HorizontalTextAlignment = TextAlignment.End;
			if (ViewModel.Indicator.IndicatorType == Constants.IndicatorTypePercentage)
			{
				label.Text = string.Format("{0}%", e.Value);
				Device.OnPlatform(iOS: () => label.FontSize = 12);
				Device.OnPlatform(Android: () => label.FontSize = 12);
				e.Label = label;
			}
			else if (ViewModel.Indicator.IndicatorType == Constants.IndicatorTypeAverage)
			{
				label.Text = string.Format("{0}", e.Value);
				Device.OnPlatform(iOS: () => label.FontSize = 12);
				Device.OnPlatform(Android: () => label.FontSize = 12);
				e.Label = label;
			}
			else {
				label.Text = string.Format("{0}", e.Value);
				Device.OnPlatform(iOS: () => label.FontSize = 12);
				Device.OnPlatform(Android: () => label.FontSize = 12);
				e.Label = label;
			}
		}
	}


}

