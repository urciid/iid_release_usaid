using System;

namespace USAID.Extensions
{
    public static class StringExtensions
    {
        public static decimal AsDecimal(this string str)
        {
            decimal dVal;
            return decimal.TryParse(str, out dVal) ? dVal : 0;
        }

        public static double AsDouble(this string str)
        {
            double dVal;
            return double.TryParse(str, out dVal) ? dVal : 0;
        }

        public static int AsInt(this string str)
        {
            int iVal;
            return int.TryParse(str, out iVal) ? iVal : 0;
        }
    }
}

