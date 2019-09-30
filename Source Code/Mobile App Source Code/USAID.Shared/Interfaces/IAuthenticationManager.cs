using System;
using System.Threading.Tasks;

namespace USAID.Interfaces
{
    public interface IAuthenticationManager
    {
        Task<bool> Authenticate();

        string GetAuthToken();
    }
}

