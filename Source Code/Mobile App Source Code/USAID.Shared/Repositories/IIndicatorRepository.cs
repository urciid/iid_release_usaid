using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IIndicatorRepository
	{
		IList<Indicator> All(Expression<Func<Indicator, bool>> filter = null,
						 Func<IQueryable<Indicator>, IOrderedQueryable<Indicator>> orderBy = null, bool recursive = false);

		void Upsert(Indicator item);
		void Delete(int id);
		void DeleteAll(Expression<Func<Indicator, bool>> filter = null, bool recursive = false);
		Indicator GetIndicatorModel(int indicatorId);
		List<ObservationEntry> GetObservationEntriesForIndicator(int indicatorId, int siteId);

	}
}

