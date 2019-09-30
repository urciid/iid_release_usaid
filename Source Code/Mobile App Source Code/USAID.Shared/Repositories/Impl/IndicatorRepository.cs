using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class IndicatorRepository : RepositoryBase<Indicator>, IIndicatorRepository
	{
		public IndicatorRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<Indicator>();
			Connection.CreateTable<IndicatorAgePeriod>();

		}
		public Indicator GetIndicatorModel(int indicatorId)
		{
			//lock (locker)
			//{
			var sql = string.Format("select * from Indicator where IndicatorId = {0}  ", indicatorId);
			var y = Connection.Query<Indicator>(sql).FirstOrDefault();
			return y;
		}
		public List<ObservationEntry> GetObservationEntriesForIndicator(int indicatoryId, int siteId)
		{
			//lock (locker)
			//{
			var sql = string.Format("select o.begin_date, o.end_date, i.Age_Range,  o.Observation_id, oe.* " +
			                        "from ObservationEntry oe \nleft join IndicatorAgePeriod i on oe.Indicator_Age_Range_Id = i.Indicator_Age_Range_Id " +
			                        "join Observation o on o.Observation_Id=oe.Observation_Id " +
			                        "where o.indicator_id = {0} and o.site_id = {1}", indicatoryId, siteId);

			var x = Connection.Query<ObservationEntry>(sql).ToList();
			return x;
			//}
		}
	}
}

