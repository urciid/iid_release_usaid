using System;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;
using USAID.Repositories;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Respositories.Impl
{
	public class SiteRepository : RepositoryBase<Site>, ISiteRepository
	{
		public SiteRepository(ISQLiteConnectionProvider provider) : base(provider)
		{
			Connection.CreateTable<Site>();
		}
	}
}

