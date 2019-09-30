using System.Threading.Tasks;
using WMP.Core.Data.SQLiteNet;
using USAID.Base;
using USAID.Interfaces;
using Xamarin.Forms;
using SQLite.Net;
using USAID.Services;
using USAID.Models;
using System.Collections.Generic;
using USAID.Pages;
using USAID.Builders;
using System.Linq;
using System;
using USAID.Extensions;
using USAID.Enumerations;

namespace USAID.ViewModels
{
	public class RateOptionsViewModel : BaseViewModel
	{
		private readonly IRateCardService _rateCardService;
        private readonly IHUDProvider _hudProvider;
		private readonly IMonthlyPaymentService _monthlyPaymentService;
		private readonly IQuoteBuilder _quoteBuilder;
        private readonly IDealerDefaultsManager _dealerDefaultsManager;


		public RateOptionsViewModel(IGALogger logger, IRateCardService rateCardService, IHUDProvider hudProvider,
            IMonthlyPaymentService monthlyPaymentService, IQuoteBuilder quoteBuilder, IDealerDefaultsManager dealerDefaultsManager)
		{
            _hudProvider = hudProvider;
			_rateCardService = rateCardService;
			_monthlyPaymentService = monthlyPaymentService;
			_quoteBuilder = quoteBuilder;
            _dealerDefaultsManager = dealerDefaultsManager;
			_quoteBuilder.CreateQuote();
		}

        public double MaxEquipmentAmount { get; set; }
        public double MinEquipmentAmount { get; set; }
		public double MaxPoints { get; set; }

		#region Properties

        public bool ShowPoints
        {
            get
            {
                return _dealerDefaultsManager.DealerDefaults.ShowPoints;
            }
        }

        public bool ShowPassThru
        {
            get
            {
                return _dealerDefaultsManager.DealerDefaults.ShowPassThruOnQuote;
            }
        }

		public string CompanyName
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string EquipmentAmount
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string EquipmentDescription
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public List<RateCardLocal> RateCards
		{
			get { return GetValue<List<RateCardLocal>>(); }
			set { SetValue(value); }
		}

		public string Points
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string PassThrough
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public IList<TermItem> Terms
		{
			get { return GetValue<IList<TermItem>>(); }
			set { SetValue(value); }
		}

		public IList<MaintenanceType> MaintenanceTypes
		{
			get { return GetValue<IList<MaintenanceType>>(); }
			set { SetValue(value); }
		}

		public IList<AdvancePayment> AdvancePayments
		{
			get { return GetValue<IList<AdvancePayment>>(); }
			set { SetValue(value); }
		}

		public IList<PurchaseOption> PurchaseOptions {
			get { return GetValue<IList<PurchaseOption>> (); }
			set { SetValue (value); }
		}

		public bool PointsVisible
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}

		public bool PassThroughVisible
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}


		#endregion


		public async Task<bool> LoadRateOptions()
		{
			var quote = _quoteBuilder.GetQuote ();
			if (quote.RateCards != null) {
				return true;
			}
			PointsVisible = true;
			PassThroughVisible = true;

			//call service to get rate card options for dealer
			try
			{
                _hudProvider.DisplayProgress("Getting Rate Cards");

				//Pass through Amount -  maintenance Amount
				//Maintenance Type - pick list

				var response = await _rateCardService.GetRateCards();
				if (response != null)
				{
                    if (response.ErrorStatusCode.IsErrorCode())
                    {
                        _hudProvider.Dismiss();
                        return false;
                    }
                    else
                    {
                        RateCards = response.RateCardsLocal;
                        if (RateCards.Count() > 0)
                        {
                            //getting terms 
                            var termList = new List<TermItem>();
                            foreach (int item in RateCards.SelectMany(x => x.Terms.Select(t => t.Term)).Distinct())
                            {
                                termList.Add(new TermItem { IsSelected = false, Term = item, TermDisplay = item.ToString() + " months" });
                            }
                            Terms = termList;
                            Terms.OrderBy(m => m.Term);

                            //getting maintenance types
                            MaintenanceTypes = RateCards.SelectMany(x => x.MaintenanceTypes).GroupBy(p => p.MaintenanceTypeDescription).Select(g => g.First()).ToList();

                            //getting purchase options
                            PurchaseOptions = RateCards.SelectMany (x => x.PurchaseOptions).GroupBy (p => p.PurchaseOptionDesc).Select (g => g.First ()).ToList();

							//getting advance payments
							AdvancePayments = RateCards.SelectMany(x => x.AdvancePayments).GroupBy(p => p.AdvancePaymentDescription).Select(g => g.First()).ToList();

							MaxPoints = RateCards.Max(rc => rc.AvailablePoints);
							MaxEquipmentAmount = RateCards.Max(rc => rc.MaximumAmount);
                            MinEquipmentAmount = RateCards.Min(rc => rc.MinimumAmount);
					    }
                    }
				}
			}
			catch (Exception ex)
			{
				
			}
            finally
            {
                _hudProvider.Dismiss();
            }

            return true;
		}

        internal async Task<RateOptionsSubmissionResult> SubmitRateOptions()
        {
            var equipmentAmount = EquipmentAmount.AsDouble();
            if (equipmentAmount > MaxEquipmentAmount || equipmentAmount < MinEquipmentAmount)
            {
                return RateOptionsSubmissionResult.InvalidEquipmentAmount;
            }
			var pointAmount = Points.AsInt();
			if (pointAmount > MaxPoints)
			{
				return RateOptionsSubmissionResult.InvalidPointAmount;
			}

			//need to get max points value
            
			var terms = Terms.Where(t => t.IsSelected);
			var rateCards = RateCards.Where(r => r.IsSelected);
			var maintenanceTypes = MaintenanceTypes.Where (m => m.IsSelected);
			var purchaseOptions = PurchaseOptions.Where(p => p.IsSelected);
			var advancePayments = AdvancePayments.Where(a => a.IsSelected);

            _quoteBuilder.SetRateOptions(new RateOptions
            {
                CompanyName = CompanyName,
                EquipmentAmount = EquipmentAmount.AsDouble(),
                EquipmentDescription = EquipmentDescription,
                RateCards = rateCards.ToList(),
                Terms = terms.ToList(),
                MaintenanceTypes = maintenanceTypes.ToList(),
                PurchaseOptions = purchaseOptions.ToList (),
				Points = Points.AsInt(),
                PassThrough = PassThrough.AsInt(),
				AdvancePayments = advancePayments.ToList()
									                                 
			});
			try
			{
                _hudProvider.DisplayProgress("Retrieving Payment Options");

				var response = await _monthlyPaymentService.GetMonthlyPayments(_quoteBuilder.GetQuote());
                if (response != null)
                {
                    if (response.ErrorStatusCode == 400)
                    {
                        return RateOptionsSubmissionResult.UnableToRetrieveData;
                    }
                    else if (response.ErrorStatusCode == 401)
                    {
                        return RateOptionsSubmissionResult.Unauthorized;
                    }
					else if (response.MonthlyPayments == null || response.MonthlyPayments.Count () == 0)
                    {
                        return RateOptionsSubmissionResult.Failure;
					}
                    else
                    {
						_quoteBuilder.SetMonthlyPayments (response.MonthlyPayments.ToList());
                        return RateOptionsSubmissionResult.Success;
					}
				}
				return RateOptionsSubmissionResult.Failure;
			}
			catch (Exception ex)
			{
				return RateOptionsSubmissionResult.Failure; //tell view to pop alert
			}
            finally
            {
                _hudProvider.Dismiss();
            }
		}
	}
}

