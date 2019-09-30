using System;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class SiteIndicatorRepository : RepositoryBase<SiteIndicator>, ISiteIndicatorRepository
	{
		public SiteIndicatorRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<SiteIndicator>();
		}
	}
}

