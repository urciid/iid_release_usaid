using System;
using USAID.Common;
using USAID.Models;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Repositories.Impl
{
	public class QuoteRepository : RepositoryBase<Quote>, IQuoteRepository
	{
		public QuoteRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<MonthlyPayment> ();
			Connection.CreateTable<TermItem> ();
			Connection.CreateTable<PurchaseOption> ();
			Connection.CreateTable<MaintenanceType> ();
			Connection.CreateTable<AdvancePayment> ();
			Connection.CreateTable<RateCardLocal> ();

		}
	}
}

