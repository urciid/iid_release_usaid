using System;
using System.Collections.Generic;
using USAID.Models;

namespace USAID.Services.ServiceModels
{
    public class SubmitCreditAppRequest
    {
        public CreditAppMetaData Application { get; set; }

        public TermData Terms { get; set; }

        public Vendor Vendor { get; set; }

        public Lessee Lessee { get; set; }

        public IEnumerable<Asset> Assets { get; set; }

        public IEnumerable<Guarantor> Guarantors { get; set; }
    }
}

