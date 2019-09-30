using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WMP.Core.Data;

namespace WMP.Core.Data.SQLiteNet
{
    public class RepositoryBase<TModel> : ISQLiteRepository<TModel> where TModel : class, IModel
    {
        private const int EXPIRATIONMINUTES = 10080;
        protected SQLiteConnection Connection { get; }

        public virtual int ExpirationMinutes
        {
            get { return EXPIRATIONMINUTES; }
        }

        protected ISQLiteConnectionProvider _connectionProvider;

        public RepositoryBase(ISQLiteConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            Connection = connectionProvider.Connection;

            // perform initial setup
            Connection.CreateTable<TModel>();
            Initialize();
        }

        public virtual List<TModel> Get(List<int> ids, bool withChildren = false, bool recursive = false)
        {
            /*
             * Note(Jackson) this method does not use getallwithchildren because we would have to write something 
             * to the effect of Connection.GetAllWithChildren(x=>ids.Contains(x.Id)); which can throw a SQLite exception if 
             * you have to many ids (the query generated has too many variables)
             *             
            
             */
            if (ids == null || ids.Count == 0)
            {
                return new List<TModel>();
            }
            try
            {
                var items = new List<TModel>();
                foreach (var id in ids)
                {
                    var item = Get(id, withChildren, recursive:recursive);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }

                return items;
            }
            catch (InvalidOperationException)
            {
                return new List<TModel>();
            }
        }

        /// <summary>
        /// This method does not do anything with deferred execution. 
        /// Calling this will bring EVERYTHING into memory. Be careful.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public virtual IList<TModel> All(Expression<Func<TModel, bool>> filter = null,
                Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, bool recursive = false)
        {
            List<TModel> list = new List<TModel>();

            if (filter != null)
            {
                list = Connection.GetAllWithChildren<TModel>(filter,
                    recursive:recursive);
            }
            else
            {
                list = Connection.GetAllWithChildren<TModel>(recursive: recursive);
            }
            
            if (orderBy != null)
            {
                return orderBy(list.AsQueryable()).ToList();
            }
            else
            {
                return list;
            }
        }
        public virtual void Upsert(TModel item)
        {
            Upsert(new List<TModel>() {item});
        }

        public virtual void Upsert(ICollection<TModel> items)
        {
            var objIds =
                items.Select(i => (object)i.Id)
                    .ToList();
           
            _connectionProvider.WaitOne();
            try
            {
                Connection.RunInTransaction(() =>
                {
                    Connection.DeleteAllIds<TModel>(objIds);
                    Connection.InsertAllWithChildren(items, recursive: true);
                });
            }
            finally
            {
                _connectionProvider.Release();
            }
            OnReplaceComplete(items);
        }

        public virtual void Delete(int id)
        {
            Connection.Delete<TModel>(id);
            Connection.Commit();
        }

        public virtual void DeleteAll(Expression<Func<TModel, bool>> filter = null, bool recursive = false)
        {
            if (recursive == false && filter == null)
            {
                Connection.DeleteAll<TModel>();
            }
            else
            {
                var modelsToDelete = All(filter);
                Connection.DeleteAll(modelsToDelete, recursive);
            }
        }

        /// <summary>
        /// This method will run the delete in a transaction. 
        /// If there is a failure none of the objects will be deleted.
        /// Each Item is deleted individually 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="recursive"></param>
        public virtual void DeleteAllInTransaction(ICollection<TModel> items, bool recursive = false)
        {
            _connectionProvider.WaitOne();
            try
            {
                Connection.RunInTransaction(() =>
                {
                    foreach (var item in items)
                    {
                        Connection.Delete(item,recursive);
                    }
                });
            }
            finally
            {
                _connectionProvider.Release();
            }
        }

        public virtual TModel Get(int id, bool withChildren = false, bool recursive = false)
        {
            try
            {
                if (!withChildren)
                {
                    var item = Connection.Get<TModel>(id);
                    return item;
                }
                else
                {
                    var item = Connection.GetWithChildren<TModel>(id, recursive);
                    return item;
                }
            }
            catch (InvalidOperationException) // If SQLite finds nothing it throws this exception
            {
                return null;
            }
        }

		//public List<TModel> GetObservations(int siteId, int IndicatorId)
		//{
		//	//lock (locker)
		//	//{
		//	var sql = string.Format("select o.Observation_id from Observation o join ObservationEntry oe on o.Observation_id = oe.Observation_id where o.Indicator_id = {0} and o.SiteId = {1} ", IndicatorId, siteId);
		//		//if (id > 0)
		//		//{
		//		//	//sql = "select Shot, sum(case when make = 1  then 1 else 0 end) Make,  sum(case when make = 0 then 1 else 0 end) Miss from SessionShot group by shot ";
		//		//	sql = string.Format("select Shot, sum(case when make = 1  then 1 else 0 end) Make,  sum(case when make = 0 then 1 else 0 end) Miss from SessionShot where SessionID = {0} group by shot ", id.ToString());

		//		//}
		//		//else {
		//		//	sql = "select Shot, sum(case when make = 1  then 1 else 0 end) Make,  sum(case when make = 0 then 1 else 0 end) Miss from SessionShot group by shot ";
		//		//}
				
		//		var x = Connection.Query<TModel>(sql).ToList();
		//		return x;
		//	//}
		//}

        public virtual void Initialize()
        {
            // by default this method will do nothing, use if you need to expand on the typical initialization process
        }

        public virtual void OnReplaceComplete(ICollection<TModel> items)
        {
            // by default this method will do nothing, but use it if you need to take action after a replace
        }
    }
}
