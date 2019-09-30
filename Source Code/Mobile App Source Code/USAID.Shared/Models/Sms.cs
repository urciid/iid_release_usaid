using System;
using System.Collections.Generic;

namespace USAID.Models
{
    public class Sms
    {
        public List<string> Recipients { get; set; }

        public string Body { get; set; }
    }
}

