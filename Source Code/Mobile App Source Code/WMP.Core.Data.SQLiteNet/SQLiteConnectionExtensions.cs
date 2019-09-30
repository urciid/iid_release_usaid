using System.Collections.Generic;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;

namespace WMP.Core.Data.SQLiteNet
{
    public static class SQLiteConnectionExtensions
    {
        public static void UpdateAllWithChildren<T>(this SQLiteConnection conn, List<T> entities)
        {
            foreach (var entity in entities)
            {
                conn.UpdateWithChildren(entity);
            }
        }
    }
}