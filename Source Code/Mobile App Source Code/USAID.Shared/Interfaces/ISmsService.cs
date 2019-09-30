using System;
using USAID.Models;

namespace USAID.Interfaces
{
    public interface ISmsService
    {
        void CreateSms(Sms sms);
        bool CanSendSms();
    }
}
