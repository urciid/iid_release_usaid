using System;
using System.Threading.Tasks;
using USAID.Services.ServiceModels;

namespace USAID.Interfaces
{
	public interface IAuthenticationService
	{
		Task<AuthenticationResponse> Authenticate(string email, string password);
//
//		Task<bool> LogoutAsync();
//
//		Task<string> GetTokenAsync();
	}
}

