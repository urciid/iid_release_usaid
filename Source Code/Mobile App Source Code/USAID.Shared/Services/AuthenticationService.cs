using USAID.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using USAID.Common;
using USAID.Interfaces;
using USAID.Services.ServiceModels;

namespace USAID.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthenticationResponse> Authenticate(string email, string password)
        {
            var authRequest = new AuthenticationRequest
            {
				username = email,
				Password = password,
				grant_type = "password"
			};

			var response = await WebServiceClientBase.PostLogin(Constants.AuthenticationUri, authRequest);
            return response;
        }
	}

	
}



