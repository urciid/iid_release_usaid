using System;
namespace USAID.Services.ServiceModels
{
    public class AuthenticationRequest
    {
        public string username { get; set; }

        public string Password { get; set; }

		public string grant_type { get; set; }
    }
}

