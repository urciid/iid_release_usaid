using System;
using System.Collections.Generic;
using USAID;
using USAID.Models;

namespace USAID.Builders
{
    public interface IQuoteBuilder
    {
        void CreateQuote();
		void SetQuote (Quote quote);
		Quote GetQuote();


		void SetRateOptions(RateOptions rateOptions);
		void SetMonthlyPayments (List<MonthlyPayment> monthlyPayments);
		//void SetContractTerms(ContractTerms contractTerms);

		bool EmailQuote();
		bool SmsQuote();
    }
}

