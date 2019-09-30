using System;
using System.Web.Caching;

using IID.BusinessLayer.Domain;

namespace IID.WebSite.Helpers
{
    public static class UserName
    {
        private static Cache Cache = System.Web.HttpRuntime.Cache;

        public static string Get(int userId)
        {
            string key = String.Concat("UserName_", userId);
            object name = Cache[key];
            if (name == null)
            {
                using (Entity context = new Entity())
                {
                    name = context.t_user.Find(userId)?.full_name;
                    if (name != null)
                        Cache.Add(
                            key, name, null, Cache.NoAbsoluteExpiration,
                            TimeSpan.FromMinutes(15), CacheItemPriority.Normal, null);
                }
            }
            return (string)name;
        }
    }
}