using System;
using USAID.Common;
using USAID.Models;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Repositories.Impl
{
    public class ProfileInfoRepository : RepositoryBase<ProfileInfo>, IProfileInfoRepository
    {
        public ProfileInfoRepository(ISQLiteConnectionProvider provider) : base(provider)
        {
        }

        public ProfileInfo GetDealerProfile()
        {
            var dealerProfileId = Constants.ProfileSqliteId;
            return Get(dealerProfileId);
        }
    }
}

