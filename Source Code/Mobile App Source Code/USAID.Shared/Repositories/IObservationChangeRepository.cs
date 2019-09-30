using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IObservationChangeRepository
	{
		IList<ObservationChange> All(Expression<Func<ObservationChange, bool>> filter = null,
						 Func<IQueryable<ObservationChange>, IOrderedQueryable<ObservationChange>> orderBy = null, bool recursive = false);

		void Upsert(ObservationChange item);
		void Delete(int id);
		void DeleteAll(Expression<Func<ObservationChange, bool>> filter = null, bool recursive = false);
	}
}



