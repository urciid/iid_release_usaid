using System.Collections.Generic;
using System.Linq;
using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class IndicatorAgeRepository : RepositoryBase<IndicatorAgePeriod>, IIndicatorAgeRepository
	{
		public IndicatorAgeRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<IndicatorAgePeriod>();
		}

		public List<IndicatorAgePeriod> GetAgePeriods(int indicatorId)
		{
			//lock (locker)
			//{
			var sql = string.Format("select * from IndicatorAgePeriod where Indicator_Id = {0}", indicatorId);

			var x = Connection.Query<IndicatorAgePeriod>(sql).ToList();
			return x;
			//}
		}
	}
}



