using System;
using USAID.Models;

namespace USAID.Interfaces
{
    public interface IDealerDefaultsManager
    {
        DealerDefaults DealerDefaults { get; }

        bool PullDealerDefaults();
    }
}

