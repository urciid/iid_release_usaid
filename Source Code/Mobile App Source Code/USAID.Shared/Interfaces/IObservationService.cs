using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USAID.Models;
using USAID.ServiceModels;

namespace USAID.Interfaces
{
	public interface IObservationService
	{
		Task<GetAllDataResponse> GetAllData(string email, int languageId);
		Task<ObservationSaveResponse> ObservationSave(string email, Observation observation);

		Task<ObservationUpdateResponse> ObservationUpdate(string email, int observationId, int indicatorId, int siteId, DateTime begin, DateTime end);
		Task<ObservationCommentResponse> ObservationCommentUpdate(string comment, int observationId, int observation_comment_id, string email);
		Task<ObservationChangeResponse> ObservationChangeUpdate(string description, int observationId, int observation_change_id, string email);
		Task<ObservationEntryUpdateResponse> ObservationEntryUpdate(int observationEntryId, int observationId, int indicatorAgeRange, string indicatorGender, 
		                                                            int numerator, int denominator, double count, double rate, bool yesNo, string email  );
		Task<ObservationAttachmentResponse> ObservationAttachmentUpdate(byte[] bytes, string attachment, string attachment_file_name, int observationId, int observation_attachment_id, string email);



	}
}

