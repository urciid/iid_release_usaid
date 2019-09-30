using System;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.Services.ServiceModels
{
    public class SubmitCreditAppResponse : HttpResponse
    {
        public int ApplicationID { get; set; }

        public MessageWrapper Messages { get; set; }
    }
}

