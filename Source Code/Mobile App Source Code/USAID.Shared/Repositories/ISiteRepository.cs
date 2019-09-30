using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface ISiteRepository
	{
		IList<Site> All(Expression<Func<Site, bool>> filter = null,
						 Func<IQueryable<Site>, IOrderedQueryable<Site>> orderBy = null, bool recursive = false);

		void Upsert(Site item);
		void Delete(int id);
		void DeleteAll(Expression<Func<Site, bool>> filter = null, bool recursive = false);

	}
}

