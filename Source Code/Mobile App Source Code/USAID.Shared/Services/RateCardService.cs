using System;
using System.Threading.Tasks;
using USAID.Common;
using USAID.Interfaces;
using USAID.Services.ServiceModels;
using USAID.Services;
using Xamarin.Forms;
using USAID.Models;
using System.Collections.Generic;
using USAID.Extensions;

namespace USAID.Services
{
    public class RateCardService : IRateCardService
    {
        public async Task<RateCardResponse> GetRateCards()
        {
            var response = await WebServiceClientBase.Get<RateCardResponse>(Constants.RateCardUri, true);
            if (response != null && !response.ErrorStatusCode.IsErrorCode())
            {
                //Need to loop through returned rate cards and create RateCardLocals
                response.RateCardsLocal = new List<RateCardLocal>();

                foreach (RateCard card in response.RateCards)
                {
                    RateCardLocal local = new RateCardLocal();
                    local.AdvancePayments = card.AdvancePayments;
                    local.AvailablePoints = card.AvailablePoints;
                    local.EquipmentType = card.EquipmentType;
                    local.EquipmentTypeDescription = card.EquipmentTypeDescription;
                    local.MaintenanceTypes = card.MaintenanceTypes;
                    local.PurchaseOptions = new List<PurchaseOption>();
                    foreach (string option in card.PurchaseOptions)
                    {
                        local.PurchaseOptions.Add(new PurchaseOption { PurchaseOptionDesc = option });
                    }
                    local.RateCardID = card.RateCardID;
                    local.RateCardName = card.RateCardName;
					local.MaximumAmount = card.MaximumAmount;
					local.MinimumAmount = card.MinimumAmount;
					local.Terms = new List<TermItem> ();
					foreach (int t in card.Terms)
                    {
						local.Terms.Add (new TermItem { Term = t });
					}

					response.RateCardsLocal.Add (local);
			    }
			}

            return response;
        }
    }
}

