using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class ObservationAttachmentRepository : RepositoryBase<ObservationAttachment>, IObservationAttachmentRepository
	{
		public ObservationAttachmentRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<ObservationAttachment>();
		}
	}
}


