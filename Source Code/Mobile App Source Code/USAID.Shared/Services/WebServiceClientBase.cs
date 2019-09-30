using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Autofac;
using USAID.ApplicationObjects;
using USAID.Common;
using USAID.Interfaces;
using USAID.ServiceModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using USAID.Services.ServiceModels;
using System.Text;

namespace USAID.Services
{
	public class WebServiceClientBase
	{
		private static readonly string authHeaderName = "Authorization";

		private static HttpClient _gfscClient;
		private static HttpClient GfscClient
		{
			get
			{
				if (_gfscClient == null)
				{
					_gfscClient = new HttpClient
					{
						BaseAddress = new Uri(Constants.ApiBaseAddress)
					};

					_gfscClient.DefaultRequestHeaders.Accept.Clear();
					_gfscClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				}
				return _gfscClient;
			}
		}


		public static async Task<T> Get<T>(string requestURI, bool addAuthHeader = false) where T : HttpResponse, new()
		{
			try //TODO: let caller handle exceptions?
			{
				if (addAuthHeader) //add header
				{
					AddAuthHeader();
				}

				var response = await GfscClient.GetAsync(requestURI).ConfigureAwait(false);

				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}

				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (!string.IsNullOrWhiteSpace(json))
					{
						return await Task.Run(() =>
							JsonConvert.DeserializeObject<T>(json)
						).ConfigureAwait(false);
					}


				}
				else
				{
					return new T
					{
						ErrorStatusCode = (int)response.StatusCode
					};
				}
			}
			catch (Exception e)
			{
				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}
				return default(T);
			}

			return default(T);
		}

		public static async Task<T> Post<V, T>(string requestURI, V requestModel, bool addAuthHeader = false) where T : HttpResponse, new()
		{
			try //TODO: let caller handle exceptions?
			{
				if (addAuthHeader) //add header
				{
					AddAuthHeader();
				}
				var model = JsonConvert.SerializeObject(requestModel);
				//var response = await GfscClient.PostAsync(requestURI, json).ConfigureAwait(false);
				var response = await GfscClient.PostAsync(requestURI, new StringContent(model, Encoding.UTF8, "application/json"));

				//var response = await GfscClient.PostAsync(requestURI, content).ConfigureAwait(false);
				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}

				if (response.IsSuccessStatusCode)
				{
					//var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					//if (!string.IsNullOrWhiteSpace(json))
					//{
					//	var returnT = JsonConvert.DeserializeObject<T>(json);
					//	return returnT;
					//}
					var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (!string.IsNullOrWhiteSpace(json))
					{
						var returnT = JsonConvert.DeserializeObject<List<T>>(json);
						return returnT[0];
					}
				}
				else
				{
					return new T
					{
						ErrorStatusCode = (int)response.StatusCode
					};
				}
			}
			catch (Exception e)
			{
				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}
				return default(T);
			}

			return default(T);
		}

		//
		public static async Task<ObservationUpdateResponse> PostObservation(string requestURI, ObservationUpdateRequest requestModel, bool addAuthHeader = false)
		{
			try //TODO: let caller handle exceptions?
			{
				if (addAuthHeader) //add header
				{
					AddAuthHeader();
				}

				var obs = JsonConvert.SerializeObject(requestModel);
				//var response = await GfscClient.PostAsync(requestURI, json).ConfigureAwait(false);
				var response = await GfscClient.PostAsync(requestURI, new StringContent(obs, Encoding.UTF8, "application/json"));

				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}

				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (!string.IsNullOrWhiteSpace(json))
					{
						var returnT = JsonConvert.DeserializeObject<List<ObservationUpdateResponse>>(json);
						return returnT[0];
					}
				}
				else
				{
					return new ObservationUpdateResponse
					{
						ErrorStatusCode = (int)response.StatusCode
					};
				}
			}
			catch (Exception e)
			{
				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}
				return new ObservationUpdateResponse();
			}

			return default(ObservationUpdateResponse);
		}

		public static async Task<ObservationAttachmentResponse> PostAttachment(string requestURI, ObservationAttachmentRequest requestModel, bool addAuthHeader = false)
		{
			try //TODO: let caller handle exceptions?
			{
				if (addAuthHeader) //add header
				{
					AddAuthHeader();
				}

				//var obs = JsonConvert.SerializeObject(requestModel);
				var content = new MultipartFormDataContent();

				content.Add(new StringContent(requestModel.observation_attachment_id.ToString()), "observation_attachment_id");
				content.Add(new StringContent(requestModel.observation_id.ToString()), "observation_id");
				content.Add(new StringContent(requestModel.attachment_file_name), "attachment_file_name");
				content.Add(new StringContent(requestModel.user_email), "user_email");
				content.Add(new StreamContent(requestModel.FileStream), "attachment_file_name", requestModel.attachment_file_name);

				//var uri = new Uri(WebApiRoot + "/api/Observation/UpdateAttachment");
				//HttpResponseMessage response = await client.PostAsync(uri, content);





				//var response = await GfscClient.PostAsync(requestURI, json).ConfigureAwait(false);
				var response = await GfscClient.PostAsync(requestURI, content);
				var responseString = response.Content.ReadAsStringAsync().Result;


				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}

				if (response.IsSuccessStatusCode)
				{
					var result = JsonConvert.DeserializeObject<ObservationAttachmentResponse[]>(responseString);
					return result[0];
					//var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					//if (!string.IsNullOrWhiteSpace(json))
					//{
					//	var returnT = JsonConvert.DeserializeObject<ObservationAttachmentResponse>(json);
					//	return returnT;
					//}
				}
				else
				{
					return new ObservationAttachmentResponse
					{
						ErrorStatusCode = (int)response.StatusCode
					};
				}
			}
			catch (Exception e)
			{
				if (addAuthHeader) //remove header
				{
					RemoveAuthHeader();
				}
				return new ObservationAttachmentResponse();
			}

			return default(ObservationAttachmentResponse);
		}

		public static async Task<AuthenticationResponse> PostLogin(string requestURI, AuthenticationRequest requestModel)
		{
			try //TODO: let caller handle exceptions?
			{

				var pairs = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>( "grant_type", "password" ),
					new KeyValuePair<string, string>( "username", requestModel.username ),
					new KeyValuePair<string, string> ( "Password", requestModel.Password )
				};
				FormUrlEncodedContent content = new FormUrlEncodedContent(pairs);
				var response = await GfscClient.PostAsync(requestURI, content).ConfigureAwait(false);


				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					if (!string.IsNullOrWhiteSpace(json))
					{
						var returnT = JsonConvert.DeserializeObject<AuthenticationResponse>(json);
						return returnT;
					}
				}
				else
				{
					return new AuthenticationResponse
					{
						ErrorStatusCode = (int)response.StatusCode
					};
				}
			}
			catch (Exception e)
			{
				return new AuthenticationResponse();
			}

			return default(AuthenticationResponse);
		}

		internal static Task Get<T>(object rateCardUri, bool v)
		{
			throw new NotImplementedException();
		}

		private static void AddAuthHeader()
		{
			//This portion was copied from the API docs (https://betaintegreat.greatamerica.com/doc/#ratecard)
			IAuthenticationManager authManager = AppContainer.Container.Resolve<IAuthenticationManager>();
			string token = authManager.GetAuthToken();
			string password = string.Empty;
			//string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("Bearer:" + username));

			GfscClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			//if (!GfscClient.DefaultRequestHeaders.Contains(authHeaderName))
			//{
			//	GfscClient.DefaultRequestHeaders.Add(authHeaderName, username);
			//}
		}

		private static void RemoveAuthHeader()
		{
			_gfscClient.DefaultRequestHeaders.Remove(authHeaderName);
		}
	}
}

