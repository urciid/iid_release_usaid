using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using WMP.Shared.ViewModels;
using WMP.Shared.Base;
using WMP.Shared.ApplicationObjects;
using Autofac;

namespace WMP.Shared.Pages

{
	/// <summary>
	/// Each ContentPage is required to align with a corresponding ViewModel
	/// ViewModels will be the BindingContext by default
	/// </summary>
	public class BaseContentPage<T> : ContentPage where T : IViewModel
	{
		readonly T _viewModel;
		public T ViewModel { get { return _viewModel; } }

		public BaseContentPage()
		{
			using (var scope = AppContainer.Container.BeginLifetimeScope())
			{
				_viewModel = AppContainer.Container.Resolve<T>();
			}
			BindingContext = _viewModel;
		}
	}


	public class BaseContentPage2<T> : MainBaseContentPage where T : BaseViewModel, new()
	{
		protected T _viewModel;

		public T ViewModel
		{
			get
			{
				return _viewModel ?? (_viewModel = new T());
			}
		}

		~BaseContentPage2()
		{
			_viewModel = null;
		}

		public BaseContentPage2()
		{
			BindingContext = ViewModel;
		}
	}

	public class MainBaseContentPage : ContentPage
	{
		bool _hasSubscribed;

		public Color BarTextColor
		{
			get;
			set;
		}

		public Color BarBackgroundColor
		{
			get;
			set;
		}

		public MainBaseContentPage()
		{
			//Debug.WriteLine("Constructor called for {0} {1}".Fmt(GetType().Name, GetHashCode()));

			BarBackgroundColor = (Color)App.Current.Resources["grayPrimary"];
			BarTextColor = Color.White;
			BackgroundColor = Color.White;

			_hasSubscribed = true;
		}


		public bool HasInitialized
		{
			get;
			private set;
		}

		protected virtual void OnLoaded()
		{
			TrackPage(new Dictionary<string, string>());
		}

		internal virtual void OnUserAuthenticated()
		{
			//App.Current.ProcessPendingPayload();
		}

		protected virtual void Initialize()
		{
		}

		protected override void OnAppearing()
		{
			if(!_hasSubscribed)
			{
				//SubscribeToAuthentication();
				//SubscribeToIncomingPayload();
				_hasSubscribed = true;
			}

			var nav = Parent as NavigationPage;
			if(nav != null)
			{
				nav.BarBackgroundColor = BarBackgroundColor;
				nav.BarTextColor = BarTextColor;
			}

			if(!HasInitialized)
			{
				HasInitialized = true;
				OnLoaded();
			}

			//App.Current.ProcessPendingPayload();
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			_hasSubscribed = false;

			base.OnDisappearing();
		}

		void OnAuthenticated()
		{
//			if(App.CurrentAthlete != null)
//			{
//				OnUserAuthenticated();
//			}
		}

		public void AddDoneButton(string text = "Done", ContentPage page = null)
		{
			var btnDone = new ToolbarItem {
				Text = text,
			};

			btnDone.Clicked += async(sender, e) =>
				await Navigation.PopModalAsync();

			page = page ?? this;
			page.ToolbarItems.Add(btnDone);
		}

		protected virtual void TrackPage(Dictionary<string, string> metadata)
		{
			var identifier = GetType().Name;
			GALogger.TrackPage(identifier, null);
		}


		#region Authentication

//		public async Task EnsureUserAuthenticated()
//		{
//			if(Navigation == null)
//				throw new Exception("Navigation is null so unable to show auth form");
//
//			var authPage = new AuthenticationPage();
//			await Navigation.PushModalAsync(authPage, true);
//
//			await Task.Delay(300);
//			var success = await authPage.AttemptToAuthenticateAthlete();
//
//			if(success && Navigation.ModalStack.Count > 0)
//			{
//				await Navigation.PopModalAsync();
//			}
//		}

//		async protected void LogoutUser()
//		{
//			var decline = await DisplayAlert("For ultra sure?", "Are you sure you want to log out?", "Yes", "No");
//
//			if(!decline)
//				return;
//
//			var authViewModel = DependencyService.Get<AuthenticationViewModel>();
//			authViewModel.LogOut(true);
//
//			App.Current.StartRegistrationFlow(); 
//		}

		#endregion
	}
}