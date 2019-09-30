using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WMP.SQLite;

namespace WMP.Core.Data.SQLiteNet
{
    public interface ISQLiteRepository<TModel> : IRepository<TModel> where TModel : IModel
    {

    }
}