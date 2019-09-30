using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IObservationAttachmentRepository
	{
		IList<ObservationAttachment> All(Expression<Func<ObservationAttachment, bool>> filter = null,
						 Func<IQueryable<ObservationAttachment>, IOrderedQueryable<ObservationAttachment>> orderBy = null, bool recursive = false);

		void Upsert(ObservationAttachment item);
		void Delete(int id);
		void DeleteAll(Expression<Func<ObservationAttachment, bool>> filter = null, bool recursive = false);
	}
}



