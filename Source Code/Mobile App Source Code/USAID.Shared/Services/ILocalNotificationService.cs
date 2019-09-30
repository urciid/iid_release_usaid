namespace USAID.Services
{

	public interface ILocalNotificationService 
	{
		void Notify(string title, string body, int id);
	}
}
