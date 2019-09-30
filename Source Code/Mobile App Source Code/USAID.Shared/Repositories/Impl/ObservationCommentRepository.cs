using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class ObservationCommentRepository : RepositoryBase<ObservationComment>, IObservationCommentRepository
	{
		public ObservationCommentRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<ObservationComment>();
		}
	}
}


