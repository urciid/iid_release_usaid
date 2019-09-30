using System;
using System.Collections.Generic;
using System.Linq;
using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class ObservationRepository : RepositoryBase<Observation>, IObservationRepository
	{
		public ObservationRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			
			Connection.CreateTable<ObservationAttachment>();
			Connection.CreateTable<Period>();


	
		}


		public List<Observation> GetObservations(int siteId, int IndicatorId)
		{
			//lock (locker)
			//{
			var sql = string.Format("select o.Id, o.Created, o.Modified, o.Title, o.Observation_id, o.Indicator_id, o.Site_id, o.Begin_Date, o.End_date from Observation o join ObservationEntry oe on o.Observation_id = oe.Observation_id where o.Indicator_id = {0} and o.Site_id = {1} ", IndicatorId, siteId);

			var x = Connection.Query<Observation>(sql).ToList();
			return x;
			//}
		}

	}
}

