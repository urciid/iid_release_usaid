using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using IID.BusinessLayer.Globalization;

namespace IID.WebSite.Helpers
{
    public static class IidCulture
    {
        private const char CultureDelimiter = '-';

        public static readonly string[] Cultures =
            new string[] { "en-US", "en-CA", "es-ES", "fr-FR", "en-GB" };

        public static bool IsRighToLeft()
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;

        }

        public static string GetImplementedCulture(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                if (Cultures.Where(c => c.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Any())
                    return name;

                var n = GetNeutralCulture(name);
                foreach (var c in Cultures)
                    if (c.StartsWith(n))
                        return c;
            }

            return Cultures.First();
        }

        public static string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture.Name;
        }

        public static string GetCurrentNeutralCulture()
        {
            return GetNeutralCulture(GetCurrentCulture());
        }

        public static string GetNeutralCulture(string name)
        {
            if (!name.Contains(CultureDelimiter))
                return name;

            return name.Split(CultureDelimiter)[0];
        }

        public static CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public static Language CurrentLanguage
        {
            get
            {
                string cultureCode = CurrentCulture.ToString();
                switch (cultureCode.Substring(0, cultureCode.IndexOf("-")))
                {
                    case "fr":
                        return Language.French;

                    case "es":
                        return Language.Spanish;

                    default:
                        return Language.English;
                }
            }
        }

        public static byte CurrentLanguageId { get { return (byte)CurrentLanguage; } }

        public sealed class AllowedCulture
        {
            public string Code { get; set; }
            public string NativeName { get; set; }

            private AllowedCulture() { }

            private AllowedCulture(string code, string nativeName)
            {
                Code = code;
                NativeName = nativeName;
            }

            private static readonly AllowedCulture[] _allowedCultures;
            public static AllowedCulture[] AllowedCultures { get { return _allowedCultures; } }

            static AllowedCulture()
            {
                var regEx = new Regex(@".*?(?=[,\)])", RegexOptions.Compiled);
                _allowedCultures =
                    IidCulture.Cultures
                        .Select(c => new AllowedCulture(c, (regEx.Match(CultureInfo.GetCultureInfo(c).NativeName).Value + ")")))
                        .OrderBy(c => c.NativeName)
                        .ToArray();
            }
        }
    }
}