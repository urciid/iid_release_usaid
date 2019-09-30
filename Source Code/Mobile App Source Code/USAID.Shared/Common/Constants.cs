using System;
using System.Collections.Generic;

namespace USAID.Common
{
	public class Constants
	{
		public static readonly string UserAuthenticated = "UserAuthenticated";
		public static readonly string GoogleAnalyticsKey = "";
		public static readonly string EmailKey = "EmailKey";
		public static readonly string PasswordKey = "PasswordKey";
		public static readonly string TokenKey = "TokenKey";
		public static readonly string LastDownloadKey = "LastDownloadKey";
        public static readonly string PhotoFilePathKey = "PhotoFilePath";
		public static readonly string CreateProfileEmailAddress = "";
		public static readonly string IndicatorTypePercentage = "Percentage";
		public static readonly string IndicatorTypeAverage = "Average";
		public static readonly string IndicatorTypeRate = "Ratio";
		public static readonly string IndicatorTypeCount = "Count";
		public static readonly string IndicatorTypeYesNo = "Yes/No";
		public static readonly string ObservationMale = "M";
		public static readonly string ObservationFeMale = "F";
		public static readonly string ObservationTotal = "Total";
		public static readonly int English = 1;
		public static readonly int Spanish = 2;
		public static readonly int French = 3;


		//period constants
		public static readonly string IndicatorFrequencyDaily = "Daily";
		public static readonly string IndicatorFrequencyWeekly = "Weekly";
		public static readonly string IndicatorFrequencyBiWeekly = "Bi-Weekly";
		public static readonly string IndicatorFrequencyMonthly = "Monthly";
		public static readonly string IndicatorFrequencyQuarterly = "Quarterly";
		public static readonly int PeriodBackAmount = 24;





		#region Screen Constants
		public static readonly string RateOptionsTitle = "Parameters";

		#endregion

        // Picker options
        // list of U.S. states/territories for credit app customer info
        // TODO: have this match current SnappShot
        public static readonly List<string> States = new List<string> { "AL", "AK", "AB", "AZ", "AR", "BC", "BV", "CA", "CN", "CO", "CT", "DE", "DC", "FL", "GA", "GU", "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MB", "MD", "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NB", "NH", "NJ", "NM", "NY", "NF", "NC", "ND", "NS", "NU", "NT", "OH", "OK", "ON", "OR", "PA", "PE", "PR", "QC", "RI", "SK", "SC", "SD", "TN", "TX", "UT", "VT", "VI", "VA", "WA", "WV", "WI", "WY", "YT" };
       
        // use a single value to get and save since we will only ever save one profile
        public static readonly int ProfileSqliteId = 1;

		//API URIs
		public static readonly string ApiBaseAddress = "https://xxxxxxxxx.azurewebsites.net/";
        public static readonly string AuthenticationUri = "Token";
		public static readonly string GetAllDataUri = "api/observation/GetAllData";
		public static readonly string OperationUpdateUri = "api/observation/Update";
		public static readonly string OperationCommentUpdateUri = "api/observation/UpdateComment";
		public static readonly string OperationChangeUpdateUri = "api/observation/UpdateChange";
		public static readonly string OperationEntryUpdateUri = "api/observation/UpdateEntry";
		public static readonly string OperationAttachmentUpdateUri = "api/observation/UpdateAttachment";
        //public static readonly string RateCardUri = "api/ratecard";
        //public static readonly string CreditAppUri = "CreditApplication";
        //public static readonly string MonthlyPaymentUri = "monthlypayment";

     
	}
}

