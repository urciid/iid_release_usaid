using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class ObservationChangeRepository : RepositoryBase<ObservationChange>, IObservationChangeRepository
	{
		public ObservationChangeRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<ObservationChange>();
		}
	}
}


