using System;
using USAID.Models;

namespace USAID.Repositories
{
    public interface IProfileInfoRepository
    {
        ProfileInfo GetDealerProfile();

        void Upsert(ProfileInfo item);
    }
}

