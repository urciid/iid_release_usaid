using System;
using System.Threading.Tasks;
using USAID.Models;
using USAID.Services.ServiceModels;

namespace USAID.Interfaces
{
    public interface ICreditAppService
    {
        Task<SubmitCreditAppResponse> SubmitCreditApp(CreditApp creditApp);
    }
}

