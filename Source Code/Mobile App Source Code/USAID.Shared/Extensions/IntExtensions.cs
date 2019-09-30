using System;

namespace USAID.Extensions
{
    public static class IntExtensions
    {
        //for determining if a http status code is a failure
        public static bool IsErrorCode(this int statusCode)
        {
            return statusCode >= 400;
        }
    }
}

