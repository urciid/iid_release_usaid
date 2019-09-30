using System;
using System.Runtime.CompilerServices;
using SQLite.Net;

namespace WMP.Core.Data.SQLiteNet
{
    public interface ISQLiteConnectionProvider
    {
        SQLiteConnection Connection { get; }
        bool WaitOne([CallerMemberName]string name = "");
        void Release([CallerMemberName]string name = "");
        void RunInTransaction(Action actionToRun);

      
    }
}