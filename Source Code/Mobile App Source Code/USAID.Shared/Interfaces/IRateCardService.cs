using System;
using System.Threading.Tasks;
using USAID.Services.ServiceModels;

namespace USAID.Interfaces
{
    public interface IRateCardService
    {
        Task<RateCardResponse> GetRateCards();
    }
}

