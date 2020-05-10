using System;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Core.Wccls.Models.Result;
using Newtonsoft.Json;
using WcclsCore.Models.Result;
using WcclsMobile.Models;
using Xamarin.Forms;

namespace WcclsMobile.Services {

	public class WcclsApiService : IWcclsApiService {

		private string API_URL = Device.RuntimePlatform == Device.Android ? "https://10.0.2.2:5002" : "https://localhost:5002";

		private const string SESSION_ID_HEADER = "SessionGuid";

		private string _paymentUrl { get; set; }

		private HttpClient _client { get; }

		public WcclsApiService() {
			HttpClientHandler handler = new HttpClientHandler();
			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => {
				if(cert.Issuer == "CN=localhost") {
					return true;
				}
				return errors == SslPolicyErrors.None;
			};
			_client = new HttpClient(handler);
		}

		public string GetPaymentUrl() {
			return _paymentUrl;
		}

		public async Task<(string error, LoginResult result)> Login(string username, string password) {
			HttpResponseMessage message;
			try {
				message = await _client.GetAsync($"{API_URL}/login?username={username}&password={password}");
			}
			catch(Exception e) {
				Type eType = e.GetType();
				return ($"An error occurred. Please check your internet connection and try again - {e.Message}", null);
			}
			string content = await message.Content.ReadAsStringAsync();
			if(!message.IsSuccessStatusCode) {
				return ($"Error - {content}", null);
			}
			LoginResult result;
			try {
				result = JsonConvert.DeserializeObject<LoginResult>(content);
			}
			catch(Exception e) {
				return ($"Error deserializing {nameof(LoginResult)} - {e.Message}.", null);
			}
			_paymentUrl = result.PaymentUrl;
			//Otherwise, it was a sucess and we now have a guid representing our session.
			return ("", result);
		}

		public Task<(string error, FinesResult result)> Fines(User user) {
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			return RunNonLoginCall(user, async () => {
				HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"{API_URL}/account/fines");
				message.Headers.Add(SESSION_ID_HEADER, user.SessionId);
				HttpResponseMessage response;
				try {
					response = await _client.SendAsync(message);
				}
				catch(Exception e) {
					return ($"An error occurred. Please check your internet connection and try again - {e.Message}", null);
				}
				string content = await response.Content.ReadAsStringAsync();
				if(!response.IsSuccessStatusCode) {
					return ($"Error - {content}", null);
				}
				FinesResult result;
				try {
					result = JsonConvert.DeserializeObject<FinesResult>(content);
				}
				catch(Exception e) {
					return ($"Error deserializing {nameof(FinesResult)} - {content}.", null);
				}
				if(result == null) {
					return ("Failed to get Fines.", null);
				}
				return ("", result);
			});
		}

		///<summary>Passthrough for all non login api calls. If the call fails for the first time, we will relog the user in. If that works, we will
		///attempt the api call one more time. This is because WCCLS backend is sessioned and limited to one session at a time.</summary>
		private async Task<(string error, T result)> RunNonLoginCall<T>(User user, Func<Task<(string error, T)>> apiCall) where T : class {
			(string errorFirst,T result) = await apiCall();
			if(string.IsNullOrEmpty(errorFirst)) {
				return ("",result);
			}
			//Failed for whatever reason, hopefully just a session problem.
			(string loginError, LoginResult loginResult) = await Login(user.Username, user.Password);
			if(!string.IsNullOrWhiteSpace(loginError)) {
				return (loginError, null);
			}
			//Login was succesful. We need to save off the new api call.
			user.SessionId = loginResult.SessionId;
			return await apiCall();
		}
	}
}
