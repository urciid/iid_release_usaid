using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.Services.ServiceModels
{
    public class MonthlyPaymentResponse : HttpResponse
    {
        public string ErrorMessage { get; set; }

        public IEnumerable<MonthlyPayment> MonthlyPayments { get; set; }
    }
}

