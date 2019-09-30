using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IQuoteRepository
	{
		IList<Quote> All(Expression<Func<Quote, bool>> filter = null,
						 Func<IQueryable<Quote>, IOrderedQueryable<Quote>> orderBy = null, bool recursive = false);

		void Upsert(Quote item);
		void Delete(int id);
	}
}

