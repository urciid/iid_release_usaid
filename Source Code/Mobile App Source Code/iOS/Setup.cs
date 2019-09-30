using Autofac;
using WMP.Core.Data.SQLiteNet;
using WMP.Core.Data.SQLiteNet.iOS;
using USAID.iOS.Providers;
using USAID.ApplicationObjects;
using USAID.Interfaces;
using WMP.Core.Data.SQLiteNet;
using UIKit;
using WMP.iOS.Services;
using USAID.Services;

namespace USAID.iOS
{
	public class Setup : AppSetup
	{
		UIApplication _app;
		public Setup(UIApplication app) 
		{
			_app = app;
		}

		protected override void RegisterDependencies(ContainerBuilder cb)
		{
			base.RegisterDependencies(cb);
            cb.RegisterType<HUDProvider>().As<IHUDProvider>();
            cb.RegisterType<DeviceSettings>().As<IDeviceSettings>();
            cb.RegisterType<UserInfoService>().As<IUserInfoService>();
			cb.RegisterType<GALogger>().As<IGALogger>();
            cb.RegisterType<SQLiteConnectionProviderIOS>().As<ISQLiteConnectionProvider>().WithParameter("filename","snappShot");
			cb.RegisterType<LocalNotificationServiceIOS>().As<ILocalNotificationService>().WithParameter("app", _app).SingleInstance();
            cb.RegisterType<EmailService>().As<IEmailService>();
            cb.RegisterType<SmsService>().As<ISmsService>();
		}
	}
}
