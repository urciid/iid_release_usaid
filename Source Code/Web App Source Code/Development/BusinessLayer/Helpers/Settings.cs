using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;

namespace IID.BusinessLayer.Helpers
{
    public static class Settings
    {
        private static NameValueCollection AppSettings = ConfigurationManager.AppSettings;

        public static string AdminEmailAddress
        {
            get { return AppSettings["AdminEmailAddress"]; }
        }

        public static string Domain
        {
            get { return AppSettings["Domain"]; }
        }

        public static int MaxFailedAttemptsBeforeLockout
        {
            get { return Convert.ToInt32(AppSettings["MaxFailedAttemptsBeforeLockout"]); }
        }

        public static int MaxFailedAccessAttemptsBeforePasswordReset
        {
            get { return Convert.ToInt32(AppSettings["MaxFailedAccessAttemptsBeforePasswordReset"]); }
        }

        public static string ResetPasswordUrl
        {
            get { return AppSettings["ResetPasswordUrl"]; }
        }

        public static string UrcChsPhoneNumber
        {
            get { return AppSettings["UrcChsPhoneNumber"]; }
        }

        public static string WebApiUrl
        {
            get { return AppSettings["WebApiUrl"]; }
        }

        public static string ObservationDefaultToleranceAverage
        {
            get { return AppSettings["Observation:DefaultTolerance:Average"]; }
        }
        public static string ObservationDefaultToleranceCount
        {
            get { return AppSettings["Observation:DefaultTolerance:Count"]; }
        }
        public static string ObservationDefaultTolerancePercentage
        {
            get { return AppSettings["Observation:DefaultTolerance:Percentage"]; }
        }
        public static string ObservationDefaultToleranceRatio
        {
            get { return AppSettings["Observation:DefaultTolerance:Ratio"]; }
        }

        public static string Environment
        {
            get { return AppSettings["Environment"]; }
        }

        public static string[] ExceptionReportRecipients
        {
            get { return AppSettings["ExceptionReportRecipients"].Split(';'); }
        }

        public static string DatabaseName
        {
            get
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["Entity"].ConnectionString.ToLower();
                string initialCatalogPart = connectionString.Split(';').First(x => x.StartsWith("initial catalog"));
                return initialCatalogPart.Substring(initialCatalogPart.IndexOf("=") + 1);
            }
        }

        public static class SendGrid
        {
            public static string Host
            {
                get { return AppSettings["SendGridHost"]; }
            }

            public static string ApiKey
            {
                get { return AppSettings["SendGridApiKey"]; }
            }

        }
        
        public static class ActiveDirectory
        {
            public static string Tenant { get { return AppSettings["aad:Tenant"]; } }
            public static string ClientId { get { return AppSettings["aad:ClientId"]; } }
            public static string ResourceId { get { return AppSettings["aad:ResourceId"]; } }
            public static string LoginUrl { get { return AppSettings["aad:LoginUrl"]; } }
            public static string Domain { get { return AppSettings["aad:Domain"]; } }
        }
    }
}
