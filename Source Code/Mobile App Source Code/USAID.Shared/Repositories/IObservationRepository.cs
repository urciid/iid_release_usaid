using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IObservationRepository
	{
		IList<Observation> All(Expression<Func<Observation, bool>> filter = null,
						 Func<IQueryable<Observation>, IOrderedQueryable<Observation>> orderBy = null, bool recursive = false);

		void Upsert(Observation item);
		void Delete(int id);
		void DeleteAll(Expression<Func<Observation, bool>> filter = null, bool recursive = false);
		List<Observation> GetObservations(int siteId, int IndicatorId);
		Observation Get(int id, bool withChildren = false, bool recursive = false);
	}
}

