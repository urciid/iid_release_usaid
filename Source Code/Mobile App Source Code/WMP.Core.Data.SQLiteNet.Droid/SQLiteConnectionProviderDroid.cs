using System;
using System.IO;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;

namespace WMP.Core.Data.SQLiteNet.Droid
{
    public class SQLiteConnectionProviderDroid : AbstractSQLiteConnectionProvider
    {
        private readonly SQLiteConnection _connection;


        public SQLiteConnectionProviderDroid(string filename) 
        {
            var platform = new SQLitePlatformAndroid();
            var sqliteFilename = string.Format("{0}.db3", filename);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            DbPath = Path.Combine(documentsPath, sqliteFilename);
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

