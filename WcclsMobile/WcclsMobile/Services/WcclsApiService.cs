﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Async;
using Core.DependencyInjection;
using Core.Wccls.Models.Request;
using Core.Wccls.Models.Result;
using Newtonsoft.Json;
using WcclsCore.Models;
using WcclsCore.Models.Result;
using WcclsMobile.Models;
using Xamarin.Forms;

namespace WcclsMobile.Services {

	public class WcclsApiService : IWcclsApiService {

		private string API_URL = Device.RuntimePlatform == Device.Android ? "https://10.0.2.2:5002" : "https://localhost:5002";

		private const string SESSION_ID_HEADER = "SessionGuid";

		private string _paymentUrl { get; set; }

		private HttpClient _client { get; }

		private IClock _clock { get; }

		///<summary>A dictionary to keep track of session information. The big problem is if two api calls come at the same time and
		///both try and refresh the session, they will refresh two sesssions and the first in will fail.</summary>
		private ConcurrentDictionary<User, SessionLock> _dictionaryUsers { get; } = new ConcurrentDictionary<User, SessionLock>();

		public WcclsApiService(IClock clock) {
			_clock = clock;
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

		public Task<(string error, HoldsResult result)> Holds(User user) {
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			return RunNonLoginCall(user, async () => {
				HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"{API_URL}/account/holds");
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
				HoldsResult result;
				try {
					result = JsonConvert.DeserializeObject<HoldsResult>(content);
				}
				catch(Exception e) {
					return ($"Error deserializing {nameof(HoldsResult)} - {content}.", null);
				}
				if(result == null) {
					return ("Failed to get Holds.", null);
				}
				return ("", result);
			});
		}

		public async Task<string> CancelHolds(User user, List<Hold> listHolds) {
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			(string error, _) = await RunNonLoginCall(user, async () => {
				HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"{API_URL}/account/holds/cancelholds");
				message.Headers.Add(SESSION_ID_HEADER, user.SessionId);
				message.Content = new StringContent(JsonConvert.SerializeObject(new CancelHoldsRequest {
					ListHoldIds = listHolds.Select(x => x.HoldId).ToList(),
					ListMetadataIds = listHolds.Select(x => x.Item.Id).ToList(),
				}), Encoding.UTF8, "application/json");
				HttpResponseMessage response;
				try {
					response = await _client.SendAsync(message);
				}
				catch(Exception e) {
					return ($"An error occurred. Please check your internet connection and try again - {e.Message}", "");
				}
				string content = await response.Content.ReadAsStringAsync();
				if(!response.IsSuccessStatusCode) {
					return ($"Error - {content}", "");
				}
				return ("", "");
			});
			return error;
		}

		public async Task<string> SuspendHolds(User user, List<Hold> listHolds, DateTime timeSuspend) {
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			(string error, _) = await RunNonLoginCall(user, async () => {
				HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"{API_URL}/account/holds/pauseholds");
				message.Headers.Add(SESSION_ID_HEADER, user.SessionId);
				message.Content = new StringContent(JsonConvert.SerializeObject(new PauseHoldsRequest {
					ListHoldIds = listHolds.Select(x => x.HoldId).ToList(),
					//We need to make sure to include the now time in the call to the backend. The backend will convert it to UTC. However,
					//if we send midnight, it may choose the wrong date.
					EndDate = timeSuspend,
					IsCurrentlyActive = listHolds[0].Status != HoldStatus.Suspended,
				}), Encoding.UTF8, "application/json");
				HttpResponseMessage response;
				try {
					response = await _client.SendAsync(message);
				}
				catch(Exception e) {
					return ($"An error occurred. Please check your internet connection and try again - {e.Message}", "");
				}
				string content = await response.Content.ReadAsStringAsync();
				if(!response.IsSuccessStatusCode) {
					return ($"Error - {content}", "");
				}
				return ("", "");
			});
			return error;
		}

		public async Task<string> ActivateHolds(User user, List<Hold> listHolds) {
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			(string error, _) = await RunNonLoginCall(user, async () => {
				HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"{API_URL}/account/holds/resumeholds");
				message.Headers.Add(SESSION_ID_HEADER, user.SessionId);
				message.Content = new StringContent(JsonConvert.SerializeObject(new ResumeHoldsRequest {
					ListHoldIds = listHolds.Select(x => x.HoldId).ToList(),
				}), Encoding.UTF8, "application/json");
				HttpResponseMessage response;
				try {
					response = await _client.SendAsync(message);
				}
				catch(Exception e) {
					return ($"An error occurred. Please check your internet connection and try again - {e.Message}", "");
				}
				string content = await response.Content.ReadAsStringAsync();
				if(!response.IsSuccessStatusCode) {
					return ($"Error - {content}", "");
				}
				return ("", "");
			});
			return error;
		}

		public async Task<string> UpdateHoldPickupLocation(User user, List<Hold> listHolds, Library newPickupLocation) {
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			(string error, _) = await RunNonLoginCall(user, async () => {
				HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"{API_URL}/account/holds/changeholdslocation");
				message.Headers.Add(SESSION_ID_HEADER, user.SessionId);
				message.Content = new StringContent(JsonConvert.SerializeObject(new ChangeHoldsLocationRequest {
					ListHoldIds = listHolds.Select(x => x.HoldId).ToList(),
					NewLocation = newPickupLocation,
				}), Encoding.UTF8, "application/json");
				HttpResponseMessage response;
				try {
					response = await _client.SendAsync(message);
				}
				catch(Exception e) {
					return ($"An error occurred. Please check your internet connection and try again - {e.Message}", "");
				}
				string content = await response.Content.ReadAsStringAsync();
				if(!response.IsSuccessStatusCode) {
					return ($"Error - {content}", "");
				}
				return ("", "");
			});
			return error;
		}

		///<summary>Passthrough for all non login api calls. If the call fails for the first time, we will relog the user in. If that works, we will
		///attempt the api call one more time. This is because WCCLS backend is sessioned and limited to one session at a time.</summary>
		private async Task<(string error, T result)> RunNonLoginCall<T>(User user, Func<Task<(string error, T)>> apiCall) where T : class {
			(string errorFirst,T result) = await apiCall();
			if(string.IsNullOrEmpty(errorFirst)) {
				return ("",result);
			}
			//Failed for whatever reason, hopefully just a session problem.
			SessionLock sessionLock = _dictionaryUsers.GetOrAdd(user, new SessionLock());
			string loginError = "";
			LoginResult loginResult;
			await sessionLock.RefreshIfNeeded(async () => {
				(loginError, loginResult) = await Login(user.Username, user.Password);
				//While still locked, lets update the session id of the user so all api calls can have it.
				if(string.IsNullOrWhiteSpace(loginError)) {
					user.SessionId = loginResult.SessionId;
				}
			});
			return await apiCall();
		}

		public Task<(string error, CheckedOutResult result)> CheckedOut(User user) {
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			return RunNonLoginCall(user, async () => {
				HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"{API_URL}/account/checkedout");
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
				CheckedOutResult result;
				try {
					result = JsonConvert.DeserializeObject<CheckedOutResult>(content);
				}
				catch(Exception e) {
					return ($"Error deserializing {nameof(CheckedOutResult)} - {content}.", null);
				}
				if(result == null) {
					return ("Failed to get checked out items.", null);
				}
				return ("", result);
			});
		}

		private class SessionLock {

			private AsyncLock _lock = new AsyncLock();

			///<summary>Indicates if this session is currently refreshing. Should only be accessed within the lock.</summary>
			private bool _isRefreshing = false;

			private List<SemaphoreSlim> _listSlims = new List<SemaphoreSlim>();

			public async Task RefreshIfNeeded(Func<Task> func) {
				bool shouldRefresh = false;
				SemaphoreSlim slim = null;
				await _lock.Lock(() => {
					if(_isRefreshing) {
						//Already refreshing. Add our slip to the classwide list while we wait for the one api call to refresh.
						slim = new SemaphoreSlim(1);
						slim.Wait();
						_listSlims.Add(slim);
					}
					else {
						//We are the ones that will refresh.
						_isRefreshing = true;
						shouldRefresh = true;
					}
				});
				if(shouldRefresh) {
					//If we're the ones refreshing, we will actually call the refresh function
					//and then free all waiting slims.
					await func();
					await _lock.Lock(() => {
						_isRefreshing = false;
						foreach(SemaphoreSlim s in _listSlims) {
							s.Release();
						}
						_listSlims = new List<SemaphoreSlim>();
					});
				}
				else {
					//We're not the ones refreshing. We simply need to wait till someone says it is time.
					await slim.WaitAsync();
				}
			}

		}
	}
}
