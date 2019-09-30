using System;
using System.Collections.Generic;
using System.Net;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.Services.ServiceModels
{
    public class AuthenticationResponse : HttpResponse
    {
        public string TokenID { get; set; }

		public string Access_Token { get; set;}

        public DateTime ExpiredDate { get; set; }

        /*schema for Message is the following:
         *   "messages": {
         *      "messages": []
         *   }
         * TODO: parse this and determine possible values
         */  
        public MessageWrapper Messages { get; set; }
    }
}

