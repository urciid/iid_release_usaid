using System;
using Autofac;
using USAID.ApplicationObjects;
using Xamarin.Forms;

namespace USAID.Base
{
	public class ViewPage<T> : ContentPage where T : IViewModel
	{
		readonly T _viewModel;
		public T ViewModel { get { return _viewModel; } }

		public ViewPage()
		{
			//this.ToolbarItems.Add(new ToolbarItem { Text = "test", Command = TestCommand });
			using (var scope = AppContainer.Container.BeginLifetimeScope())
			{
				_viewModel = AppContainer.Container.Resolve<T>();
			}
			BindingContext = _viewModel;
		}
	}
}

