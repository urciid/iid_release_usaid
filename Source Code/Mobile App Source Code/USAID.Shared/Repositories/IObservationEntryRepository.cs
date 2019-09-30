using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IObservationEntryRepository
	{
		IList<ObservationEntry> All(Expression<Func<ObservationEntry, bool>> filter = null,
						 Func<IQueryable<ObservationEntry>, IOrderedQueryable<ObservationEntry>> orderBy = null, bool recursive = false);

		void Upsert(ObservationEntry item);
		void Delete(int id);
		void DeleteAll(Expression<Func<ObservationEntry, bool>> filter = null, bool recursive = false);
		List<ObservationEntry> GetObservationEntries(int observationId);
	}
}


