using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WMP.Core.Data.SQLiteNet;

namespace USAID.Models
{
    public class MonthlyPayment : IModel
    {

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[ForeignKey (typeof (Quote))]
		public int QuoteId { get; set; }

		[ManyToOne (CascadeOperations = CascadeOperation.CascadeRead)]
		public Quote Quote { get; set; }

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

        public double AdvancementPayment { get; set; }

        public double Payment { get; set; }

        public string PurchaseOption { get; set; }

        public int RateCardID { get; set; }

        public string RateCardName { get; set; }

        public double Term { get; set; }

		public string MonthlyPaymentLine1 {

			get {
				return string.Format ("{0} for {1} months.", string.Format ("{0:C}", Payment) , Term);
			}

		}

		public string MonthlyPaymentLine2 {

			get {
				return string.Format ("with {0} advanced payments.", AdvancementPayment);
			}
		}

		public string TermShortDisplay {

			get {
				return string.Format ("{0}mo", Term);
			}

		}

		public bool IsSelected { get; set;}
    }
}

