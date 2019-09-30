using System;
using System.IO;
using SQLite.Net;
using WMP.Core.Data.SQLiteNet;


namespace WMP.Core.Data.SQLiteNet.iOS
{
    public class SQLiteConnectionProviderIOS : AbstractSQLiteConnectionProvider
    {
        private readonly SQLiteConnection _connection;

      
        public SQLiteConnectionProviderIOS(string filename) 
        {
            var platform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
            var sqliteFilename = string.Format("{0}.db3", filename);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            DbPath = Path.Combine(libraryPath, sqliteFilename);
            // Create the connection
            _connection = new SQLiteConnection(platform, DbPath);
        }

        public override SQLiteConnection Connection
        {
            get
            {
                return _connection;
            }
        }


        //taken from https://developer.xamarin.com/guides/xamarin-forms/working-with/databases/
        public SQLiteConnection GetConnection()
        {
            return _connection;
        }
    }
}

