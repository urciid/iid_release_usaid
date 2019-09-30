using System;
using System.Collections.Generic;
using System.Windows.Input;
using USAID.Builders;
using USAID.Extensions;
using USAID.Interfaces;
using USAID.Models;
using USAID.Repositories;
using USAID.Utilities;

namespace USAID.ViewModels
{
    public class SubmissionConfirmationViewModel : BaseViewModel
    {
        private readonly IEmailService _emailService;
        private readonly ICreditAppBuilder _creditAppBuilder;

        public string AppIdText
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public SubmissionConfirmationViewModel(IEmailService emailService, ICreditAppBuilder creditAppBuilder)
        {
            _emailService = emailService;
            _creditAppBuilder = creditAppBuilder;
            AppIdText = "Application ID: " + _creditAppBuilder.GetCreditApp().AppId;
        }

        internal void SendConfirmationEmail()
        {
            var app = _creditAppBuilder.GetCreditApp();

            _emailService.CreateEmail(app.ToConfirmationEmail());
        }
    }
}

