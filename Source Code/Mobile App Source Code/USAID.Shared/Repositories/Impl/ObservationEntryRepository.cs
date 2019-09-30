using System.Collections.Generic;
using System.Linq;
using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class ObservationEntryRepository : RepositoryBase<ObservationEntry>, IObservationEntryRepository
	{
		public ObservationEntryRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			
		}

		public List<ObservationEntry> GetObservationEntries(int observationId)
		{
			//lock (locker)
			//{
			var sql = string.Format("select oe.ObservationEntryId, Observation_id, oe.Indicator_Age_Range_Id, i.Age_Range, Indicator_Gender, Numerator, Denominator, Count, Rate, YesNo from ObservationEntry oe left join IndicatorAgePeriod i on oe.Indicator_Age_Range_Id = i.Indicator_Age_Range_Id where oe.Observation_Id = {0}",  observationId);

			var x = Connection.Query<ObservationEntry>(sql).ToList();
			return x;
			//}
		}
	}
}


