using System;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class ActivityRepository : RepositoryBase<ActivityModel>, IActivityRepository
	{
		public ActivityRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<ActivityModel>();
		}

		public ActivityModel GetActivityModel(int ActivityId)
		{
			//lock (locker)
			//{
			var sql = string.Format("select Id, Created, Modified, ActivityId, Activity, Project, Organization, Country, ActivityManager from ActivityModel where ActivityId = {0}  ", ActivityId);
			var y = Connection.Query<ActivityModel>(sql).FirstOrDefault();
			return y;
		}
	}
}

