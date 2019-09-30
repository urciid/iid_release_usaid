using System;
using System.Collections.Generic;

using Xamarin.Forms;
using USAID;
using System.Threading.Tasks;
using Plugin.Connectivity;
using USAID.Models;
using WMP.Core.Data;
using WMP.SQLite;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Common;
using USAID.Interfaces;
using USAID.Pages;
using USAID.Resx;
using System.Reflection;

namespace USAID
{
	public partial class App : Application
	{
		public static int AnimationSpeed = 250;

        private IHUDProvider _hudProvider;
        private IUserInfoService _userInfoService;
        private IAuthenticationManager _authManager;
		private ILocale _localeService;

		#region Properties

		static Application app;
		public static Application CurrentApp
		{
			get { return app; }
		}

		public static RootPage PageRoot;

        public static INavigation Navigation
        {
            get
            {
                return CurrentApp.MainPage.Navigation;
            }
        }

        public static bool IsNetworkReachable { get; set; }

		#endregion
		public App (AppSetup setup)
		{
			app = this;

            //This registers the dependences for Autofac
			AppContainer.Container = setup.CreateContainer();

            Setup();

            ShowLandingScreen();
        }

	public static bool LanguageID { get; set;}

        // Initial setup
        public void Setup()
        {
            InitializeComponent();


			var assembly = typeof(ActivitiesPage).GetTypeInfo().Assembly; // "EmbeddedImages" should be a class in your app
			foreach (var res in assembly.GetManifestResourceNames())
			{
				System.Diagnostics.Debug.WriteLine("found resource: " + res);
			}



            _hudProvider = AppContainer.Container.Resolve<IHUDProvider>();
            _userInfoService = AppContainer.Container.Resolve<IUserInfoService>();
            _authManager = AppContainer.Container.Resolve<IAuthenticationManager>();
			var ci = DependencyService.Get<ILocale>().GetCurrentCultureInfo();

            AppContainer.Container.Resolve<IDeviceSettings>().LoadInfo();

			//localization
			L10n.SetLocale(ci);
			AppResources.Culture = ci;
        }

		private async Task AuthenticateAndLaunch(bool onStart = false)
		{
			Xuni.Forms.Core.LicenseManager.Key = License.Key;
           
            _hudProvider.DisplayProgress("Authenticating");

			var userInfo = AppContainer.Container.Resolve<IUserInfoService>().GetSavedInfo();
			string token = userInfo.Token;
			bool authenticated = false;
			//Need to check if have token ID
			if (!string.IsNullOrEmpty(token))
			{
				authenticated = true;
			}

			if (authenticated)
			{
                if (onStart)
                {
                    ShowHomeScreen();
                }
			}
			else
			{
                var keysInvalidated = false;

				//check to see if GUID is valid
                var userProfile = _userInfoService.GetSavedInfo();
				if (!string.IsNullOrWhiteSpace(userProfile.Email) || 
				    !string.IsNullOrWhiteSpace(userProfile.Password))
                {
                    //user has logged in before but keys are no longer valid
                    keysInvalidated = false;
                }
				ShowLoginScreen(keysInvalidated);
			}

            _hudProvider.Dismiss();
        }

		protected override async void OnResume()
		{
			base.OnResume();
			await AuthenticateAndLaunch();
		}

        protected override async void OnStart()
        {
            base.OnStart();
			await AuthenticateAndLaunch(true);
           
        }

        // This is the screen we will show while making the authentication call
        // (since authentication affects which screen we show)
        public static void ShowLandingScreen()
        {
            CurrentApp.MainPage = new LandingPage();
        }

		public static NavigationPage GetNavigationPage(Page page)
		{
			var navigationPage = new NavigationPage(page);

			navigationPage.BarBackgroundColor = Color.FromHex("#034468");
			navigationPage.BarTextColor = Color.White;

			return navigationPage;
		}


		// User has not yet been authenticated, must log in
        public static void ShowLoginScreen(bool keysInvalidated)
		{
			AppContainer.Container.Resolve<IUserInfoService>().ClearUserInfo();
			CurrentApp.MainPage = new LoginPage(keysInvalidated);
		}

		// User has already been authenticated, log in automatically
		public static void ShowHomeScreen()
		{
			//CurrentApp.MainPage = new RootTabPage();
			PageRoot = new RootPage(); 

			CurrentApp.MainPage = PageRoot;

		}

		public static async Task ExecuteIfConnected(Func<Task> actionToExecuteIfConnected)
		{
			if (IsConnected)
			{
				await actionToExecuteIfConnected();
			}
			else
			{
				await ShowNetworkConnectionAlert();
			}
		}
		static async Task ShowNetworkConnectionAlert()
		{
			await CurrentApp.MainPage.DisplayAlert(
				"Network Error", 
				"No Network Connection", "Cancel");
		}
		public static bool isDirty { get; set;}
		public static bool IsConnected
		{
			get { return CrossConnectivity.Current.IsConnected; }
		}
	}
}

