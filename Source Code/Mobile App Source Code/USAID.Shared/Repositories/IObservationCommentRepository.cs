using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IObservationCommentRepository
	{
		IList<ObservationComment> All(Expression<Func<ObservationComment, bool>> filter = null,
						 Func<IQueryable<ObservationComment>, IOrderedQueryable<ObservationComment>> orderBy = null, bool recursive = false);

		void Upsert(ObservationComment item);
		void Delete(int id);
		void DeleteAll(Expression<Func<ObservationComment, bool>> filter = null, bool recursive = false);
	}
}



