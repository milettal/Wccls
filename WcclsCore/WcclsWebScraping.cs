using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WcclsCore {

	public class WcclsWebScraping {

		///<summary>The URL for the login method for WCCLS.</summary>
		private const string LOGIN_URL = "https://wccls.bibliocommons.com/user/login?destination=%2Fuser_dashboard";
		///<summary>The URL for the global activity api call. This is how we can tell if the user successfully logged in or not.</summary>
		private const string GLOBAL_ACTIVITY = "https://wccls.bibliocommons.com/user/1456585467/privacy_settings/global_activity_feed";
		///<summary>The URL for the fines portion of the borrowing object.</summary>
		private const string BORROWING_FINES_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=fines";
		///<summary>The URL for the checked out portion of the borrowing object.</summary>
		private const string BORROWING_CHECKED_OUT_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=checkedout";
		///<summary>The URL for the holds portion of the borrowing object.</summary>
		private const string BORROWING_HOLDS_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=holds";
		///<summary>The URL for the recentely returned portion of the borrowing object.</summary>
		private const string BORROWING_RECENTLY_RETURNED_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=recently_returned";
		///<summary>The key for the session id in the Set-Cookie response.</summary>
		private const string SESSION_ID_KEY = "_live_bcui_session_id";
		///<summary>The singleton instance for this HttpClient.</summary>
		private static HttpClient _client { get; }
		///<summary>The cookie container for the client.</summary>
		private static CookieContainer _cookieContainer { get; }

		static WcclsWebScraping() {
			_cookieContainer = new CookieContainer();
			_client = new HttpClient(new HttpClientHandler { CookieContainer = _cookieContainer });
		}

		///<summary>Attempts to log the user in with the given username or password. If this fails, it will return an empty string.</summary>
		///<param name="username">The username for this account.</param>
		///<param name="password">The password for this account.</param>
		///<exception cref="ArgumentException">Thrown if the username or password are blank.</exception>
		///<returns>The access token for this login.</returns>
		public async Task<string> Login(string username, string password) {
			if(string.IsNullOrWhiteSpace(username)) {
				throw new ArgumentException("Username is required.", nameof(username));
			}
			if(string.IsNullOrWhiteSpace(password)) {
				throw new ArgumentException("Password is required.", nameof(password));
			}
			Dictionary<string, string> formData = new Dictionary<string, string>();
			formData["utf8"] = "✓";
			formData["name"] = username;
			formData["user_pin"] = password;
			formData["local"] = "false";
			HttpResponseMessage loginResponse = await _client.PostAsync(LOGIN_URL, new FormUrlEncodedContent(formData));
			if(!loginResponse.IsSuccessStatusCode) {
				return "";
			}
			HttpResponseMessage globalFeedResponse = await _client.GetAsync(GLOBAL_ACTIVITY);
			globalFeedResponse.EnsureSuccessStatusCode();
			try {
				string json = await globalFeedResponse.Content.ReadAsStringAsync();
				var globalFeed = JsonConvert.DeserializeAnonymousType(json, new {
					logged_in = false,
					success = false,
				});
				//Reporting us as not logged in.
				if(!globalFeed.logged_in || !globalFeed.success) {
					return "";
				}
			}
			catch(Exception e) {
				//TODO: log.
				//Couldn't deserialize the JSON meaning the login failed.
				return "";
			}
			return HttpUtils.GetSetCookieParams(loginResponse)
				.Where(x => x.Name == SESSION_ID_KEY)
				.Select(x => x.Value)
				.FirstOrDefault()??"";
		}

		///<summary>Returns the borrowing object for the logged in user. Will return null if any part of the object fails.</summary>
		public async Task<Borrowing> GetBorrowing() {
			Borrowing borrowingComplete = new Borrowing();
			bool anyFailed = false;
			async Task<Borrowing> GetBorrow(string url) {
				try {
					HttpResponseMessage response = await _client.GetAsync(url);
					string json = await response.Content.ReadAsStringAsync();
					Borrowing borrow = JsonConvert.DeserializeObject<Borrowing>(json);
					return borrow;
				}
				catch(Exception e) {
					anyFailed = true;
					return null;
				}
			}
			List<Func<Task>> listTasks = new List<Func<Task>>();
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_CHECKED_OUT_URL);
				borrowingComplete.CheckedOut = borrow?.CheckedOut;
				borrowingComplete.ComingDue = borrow?.ComingDue;
				borrowingComplete.Overdue = borrow?.Overdue;
			});
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_HOLDS_URL);
				borrowingComplete.HoldsReady = borrow?.HoldsReady;
				borrowingComplete.HoldsInTransit = borrow?.HoldsInTransit;
				borrowingComplete.TotalHolds = borrow?.TotalHolds;
			});
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_RECENTLY_RETURNED_URL);
				borrowingComplete.RecentlyReturnedEnabled = borrow?.RecentlyReturnedEnabled??false;
				borrowingComplete.RecentlyReturnedLastDateCount = borrow?.RecentlyReturnedLastDateCount;
				borrowingComplete.RecentlyReturnedTotalCount = borrow?.RecentlyReturnedTotalCount;
			});
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_FINES_URL);
				borrowingComplete.Fines = borrow?.Fines??0;
			});
			await TaskUtils.WhenAll(listTasks);
			return anyFailed ? null : borrowingComplete;
		}
	}

}
