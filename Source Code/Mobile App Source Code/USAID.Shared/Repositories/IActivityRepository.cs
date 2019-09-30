using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using USAID.Models;

namespace USAID.Repositories
{
	public interface IActivityRepository
	{
		IList<ActivityModel> All(Expression<Func<ActivityModel, bool>> filter = null,
						 Func<IQueryable<ActivityModel>, IOrderedQueryable<ActivityModel>> orderBy = null, bool recursive = false);

		void Upsert(ActivityModel item);
		void Delete(int id);
		void DeleteAll(Expression<Func<ActivityModel, bool>> filter = null, bool recursive = false);
		ActivityModel GetActivityModel(int ActivityId);
	}
}

