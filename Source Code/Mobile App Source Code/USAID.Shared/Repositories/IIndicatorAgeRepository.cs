using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IIndicatorAgeRepository
	{
		IList<IndicatorAgePeriod> All(Expression<Func<IndicatorAgePeriod, bool>> filter = null,
						 Func<IQueryable<IndicatorAgePeriod>, IOrderedQueryable<IndicatorAgePeriod>> orderBy = null, bool recursive = false);

		void Upsert(IndicatorAgePeriod item);
		void Delete(int id);
		void DeleteAll(Expression<Func<IndicatorAgePeriod, bool>> filter = null, bool recursive = false);
		List<IndicatorAgePeriod> GetAgePeriods(int indicatorId);
	}
}




