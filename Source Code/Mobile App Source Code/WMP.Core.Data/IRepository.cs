using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WMP.SQLite
{
    public interface IRepository<TModel>
    {
        int ExpirationMinutes { get; }
        void Initialize();

        IList<TModel> All(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, bool recursive = false);
        void Delete(int id);
        void DeleteAll(Expression<Func<TModel, bool>> filter = null, bool recursive = false);
        void DeleteAllInTransaction(ICollection<TModel> items, bool recursive = false);

        List<TModel> Get(List<int> ids, bool withChildren = false, bool recursive = false);
        TModel Get(int id, bool withChildren = false, bool recursive = false);
        void OnReplaceComplete(ICollection<TModel> items);
        void Upsert(TModel item);
        void Upsert(ICollection<TModel> items);
    }
}