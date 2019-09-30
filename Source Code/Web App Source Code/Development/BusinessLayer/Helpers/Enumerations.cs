using System;

namespace IID.BusinessLayer.Helpers
{
    public static class Enumerations
    {
        public static T Parse<T>(string value) where T : struct
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value.Replace("-", "").Replace("/", ""));
            }
            catch(Exception)
            {
                return default(T);
            }
        }

        public static T Parse<T>(int value) where T : struct
        {
            return Parse<T>(value.ToString());
        }
    }
}
