using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface ISiteIndicatorRepository
	{
		IList<SiteIndicator> All(Expression<Func<SiteIndicator, bool>> filter = null,
						 Func<IQueryable<SiteIndicator>, IOrderedQueryable<SiteIndicator>> orderBy = null, bool recursive = false);

		void Upsert(SiteIndicator item);
		void Delete(int id);
		void DeleteAll(Expression<Func<SiteIndicator, bool>> filter = null, bool recursive = false);

	}
}

