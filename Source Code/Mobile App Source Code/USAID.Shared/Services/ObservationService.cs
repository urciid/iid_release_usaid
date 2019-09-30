using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using USAID.Interfaces;
using USAID.Services;
using USAID.Common;
using USAID.Extensions;
using USAID.ServiceModels;
using System;
using USAID.Models;
using System.IO;

namespace USAID.Services
{
	public class ObservationService : IObservationService
	{
		public async Task<GetAllDataResponse> GetAllData(string email, int languageId)
		{

			var request = new GetAllDataRequest
			{
				email = email,
				languageId = languageId
			};
			//var response = await WebServiceClientBase.Get<GetAllDataRequest, GetAllDataResponse>(Constants.GetAllDataUri, request, true);

			//return response;

			var response = await WebServiceClientBase.Get<GetAllDataResponse>(Constants.GetAllDataUri + "?email=" + email + "&languageId=" + languageId, true);
			return response;
		}
		public async Task<ObservationSaveResponse> ObservationSave(string email, Observation observation)
		{
			//need to call all the saves
			ObservationSaveResponse returnValue = new ObservationSaveResponse();

			//observation
			var request = new ObservationUpdateRequest
			{
				user_email = email,
				observation_id = observation.Observation_id,
				indicator_id = observation.Indicator_id,
				site_id = observation.Site_id,
				begin_date = (System.DateTime)observation.Begin_Date,
				end_date = (System.DateTime)observation.End_Date
			};


			var response = await WebServiceClientBase.Post<ObservationUpdateRequest, ObservationUpdateResponse>(Constants.OperationUpdateUri, request, true);
			if (response != null)
			{
				
				observation.Observation_id = response.observation_id;
				observation.ModifiedLocally = false;

				//save observation entry
				foreach (ObservationEntry entry in observation.ObservationEntries)
				{

					var entryRes = await ObservationEntryUpdate(entry.ObservationEntryId==null ? 0 : (int)entry.ObservationEntryId, 
					                                            observation.Observation_id, 
					                                            entry.Indicator_Age_Range_Id == null ? 0 : (int)entry.Indicator_Age_Range_Id, 
					                                            entry.Indicator_Gender, 
					                                            entry.Numerator == null ? 0 : (int)entry.Numerator,
					                                            entry.Denominator == null ? 0 :(int)entry.Denominator, 
					                                            entry.Count == null ? 0 :(int)entry.Count, 
					                                            entry.Rate == null ? 0 :(int)entry.Rate, 
					                                            entry.Yes_No == null ? false :(bool)entry.Yes_No, 
					                                            email);

					if (entryRes != null)
					{
						entry.ObservationEntryId = entryRes.observation_entry_id;
						entry.ModifiedLocally = false;
					}

				}

				//save observation change
				foreach (ObservationChange change in observation.Changes)
				{

					var changeRes = await ObservationChangeUpdate(change.Description, observation.Observation_id, change.ChangeId, email);

					if (changeRes != null)
					{
						change.ChangeId = changeRes.observation_change_id;
						change.ModifiedLocally = false;
					}
				}

				//save observation comment
				foreach (ObservationComment comment in observation.Comments)
				{

					var commentRes = await ObservationCommentUpdate(comment.Comment, observation.Observation_id, comment.CommentId, email);

					if (commentRes != null)
					{
						comment.CommentId = commentRes.observation_comment_id;
						comment.ModifiedLocally = false;
					}
				}

				//save observation attachment
				foreach (ObservationAttachment attach in observation.Attachments)
				{


					//byte[] file = new byte[attach.Bytes.Length];
					//int result = fileBytes.InputStream.Read(file, 0, fileBytes.ContentLength);
					//attachment.attachment = file;
					//attachment.approved = false;
					//attachment.createdby_userid = CurrentUser.Id;
					//attachment.created_date = DateTime.Now;
					//db.t_observation_attachment.Add(attachment);
					//db.SaveChanges();
					var buffer = attach.Bytes;
					if (buffer != null)
					{
						string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

						var attachRes = await ObservationAttachmentUpdate(attach.Bytes, s, attach.Attachment_File_Name, observation.Observation_id, attach.AttachmentId, email);

						if (attachRes != null)
						{
							attach.AttachmentId = attachRes.observation_attachment_id;
							attach.ModifiedLocally = false;
						}
					}

				}
			}
			else {
				returnValue.ErrorStatusCode = 500;
			}

			returnValue.observation = observation;
			return returnValue;
		}

		public async Task<ObservationUpdateResponse> ObservationUpdate(string email, int observationId, int indicatorId, int siteId, DateTime begin, DateTime end)
		{
			var request = new ObservationUpdateRequest
			{
				user_email = email,
				observation_id = observationId,
				indicator_id = indicatorId,
				site_id = siteId,
				begin_date = begin,
				end_date = end
			};
			var response = await WebServiceClientBase.Post<ObservationUpdateRequest, ObservationUpdateResponse>(Constants.OperationUpdateUri, request, true);                                        
			return response;
		}

		public async Task<ObservationCommentResponse> ObservationCommentUpdate(string comment, int observationId, int observation_comment_id, string email)
		{
			var request = new ObservationCommentRequest
			{
				comment = comment,
				user_email = email,
				observation_id = observationId,
				observation_comment_id = observation_comment_id
			};
			var response = await WebServiceClientBase.Post<ObservationCommentRequest, ObservationCommentResponse>(Constants.OperationCommentUpdateUri, request, true);
			return response;
		}

		public async Task<ObservationChangeResponse> ObservationChangeUpdate(string description, int observationId, int observation_change_id, string email)
		{
			var request = new ObservationChangeRequest
			{
				description = description,
				user_email = email,
				observation_id = observationId,
				observation_change_id = observation_change_id
			};
			var response = await WebServiceClientBase.Post<ObservationChangeRequest, ObservationChangeResponse>(Constants.OperationChangeUpdateUri, request, true);
			return response;
		}


		public async Task<ObservationAttachmentResponse> ObservationAttachmentUpdate(byte[] bytes, string attachment, string attachment_file_name, int observationId, int observation_attachment_id, string email)
		{
			var request = new ObservationAttachmentRequest
			{
				attachment = attachment,
				attachment_file_name = attachment_file_name,
				observation_attachment_id = observation_attachment_id,
				observation_id = observationId,
				user_email = email,
				FileStream = new MemoryStream(bytes)
			};
			var response = await WebServiceClientBase.PostAttachment(Constants.OperationAttachmentUpdateUri, request, true);
			//var response = await WebServiceClientBase.PostAttachment<ObservationAttachmentRequest, ObservationAttachmentResponse>(Constants.OperationAttachmentUpdateUri, request, true);
			return response;
		}

		public async Task<ObservationEntryUpdateResponse> ObservationEntryUpdate(int observationEntryId, int observationId, int indicatorAgeRange, string indicatorGender,
		                                                                         int numerator, int denominator, double count, double rate, bool yesNo, string email)
		{
			var request = new ObservationEntryUpdateRequest
			{
				count = (int)count,
				denominator = denominator,
				indicator_age_range_id = indicatorAgeRange,
				indicator_gender = indicatorGender,
				numerator = numerator,
				observation_entry_id = observationEntryId,
				observation_id = observationId,
				rate = rate,
				user_email = email,
				yes_no = yesNo
			};
			var response = await WebServiceClientBase.Post<ObservationEntryUpdateRequest, ObservationEntryUpdateResponse>(Constants.OperationEntryUpdateUri, request, true);
			return response;
		}
	}
}

