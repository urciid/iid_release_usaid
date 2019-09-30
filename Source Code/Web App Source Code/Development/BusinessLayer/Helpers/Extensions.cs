using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace IID.BusinessLayer.Helpers
{
    public static class Extensions
    {
        private static string GetClaimValue(IPrincipal user, string type)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claim = ((ClaimsIdentity)user.Identity).Claims.Where(c => c.Type == type);
                if (claim != null && claim.Any())
                    return claim.First().Value;
            }

            return String.Empty;
        }

        public static string FirstName(this IPrincipal user) { return GetClaimValue(user, "FirstName"); }

        public static string LastName(this IPrincipal user) { return GetClaimValue(user, "LastName"); }

        public static int Id(this IPrincipal user) { return Convert.ToInt32(GetClaimValue(user, "Id")); }

        public static string StringId(this IPrincipal user) { return GetClaimValue(user, "Id"); }



        //public static DateTime GetPreviousQuarterBegin(this DateTime date)
        //{
        //    return new DateTime(
        //        date.Year - (date.Month < 4 ? 1 : 0),
        //        date.Month < 4 ? 10 : date.Month < 7 ? 1 : date.Month < 10 ? 4 : 7,
        //        1); 
        //}

        //public static DateTime GetPreviousQuarterEnd(this DateTime date)
        //{
        //    return new DateTime(
        //        date.Year - (date.Month < 4 ? 1 : 0),
        //        date.Month < 4 ? 12 : date.Month < 7 ? 3 : date.Month < 10 ? 6 : 9,
        //        date.Month < 7 ? 31 : 30);
        //}

        //public static DateTime GetThisQuarterEnd(this DateTime date)
        //{
        //    int result;
        //    return new DateTime(
        //        date.Year,
        //        Math.DivRem(date.Month + 12, 3, out result),
        //        new int[] { 3, 12 }.Contains(date.Month) ? 31 : 30);
        //}



        public static ICollection<Dictionary<string, object>> ToCollection(this DataTable table)
        {
            return table.Select().Select(x =>
                x.ItemArray.Select((a, i) => new { Name = table.Columns[i].ColumnName, Value = a })
                .ToDictionary(a => a.Name, a => a.Value)).ToList();
        }

        public static Dictionary<string, Dictionary<string, object>> ToDictionaryOfDictionaries(this DataTable table, Func<Dictionary<string, object>, string> outerKey)
        {
            return table.Select().Select(x =>
                x.ItemArray.Select((a, i) => new { Name = table.Columns[i].ColumnName, Value = a })
                .ToDictionary(a => a.Name, a => a.Value))
                .ToDictionary(b => outerKey(b), b => b);
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            var list = new List<TSource>(count);
            foreach (var item in source)
                list.Add(item);
            return list;
        }
    }
}
