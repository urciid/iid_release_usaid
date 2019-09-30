using System;
using SQLite.Net;

namespace USAID.Interfaces
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}

