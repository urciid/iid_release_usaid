using USAID.Interfaces;
using USAID.Services;
using SQLite.Net;
using WMP.Core.Data.SQLiteNet;

namespace USAID.ViewModels
{
	public class HomeViewModel : BaseViewModel
	{
		
		private ILocalNotificationService _localNotificationService;

		private SQLiteConnection _connection;

		public string DealerName
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public HomeViewModel(IGALogger logger, ISQLiteConnectionProvider provider, ILocalNotificationService localNotificationService)
		{
			_connection = provider.Connection;
			_localNotificationService = localNotificationService;
		}


	}
}

