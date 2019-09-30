using Autofac;
using WMP.Core.Data.SQLiteNet;
using WMP.Core.Data.SQLiteNet.Droid;
using GA.Droid.Providers;
using USAID.ApplicationObjects;
using USAID.Interfaces;
using USAID.Services;
using GA.Droid.Services;

namespace GA.Droid
{
	public class Setup : AppSetup
	{
		protected override void RegisterDependencies(ContainerBuilder cb)
		{
			base.RegisterDependencies(cb);

            cb.RegisterType<HUDProvider>().As<IHUDProvider>();
            cb.RegisterType<DeviceSettings>().As<IDeviceSettings>();
            cb.RegisterType<UserInfoService>().As<IUserInfoService>();
            cb.RegisterType<GALogger>().As<IGALogger>();
            cb.RegisterType<SQLiteConnectionProviderDroid>().As<ISQLiteConnectionProvider>().WithParameter("filename","snappShot");
			cb.RegisterType<LocalNotificationServiceDroid>().As<ILocalNotificationService>().SingleInstance();
            cb.RegisterType<EmailService>().As<IEmailService>();
            cb.RegisterType<SmsService>().As<ISmsService>();
		}
	}
}
