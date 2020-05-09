using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WcclsMobile.Services {

	public class WcclsApiService : IWcclsApiService {

		private const string API_URL = "https://10.0.2.2:5002";

		private HttpClient _client { get; } = new HttpClient();

		public async Task<(string error, string sessionGuid)> Login(string username, string password) {
			HttpResponseMessage message;
			try {
				message = await _client.GetAsync($"{API_URL}/login?username={username}&password={password}");
			}
			catch(Exception e) {
				Type eType = e.GetType();
				return ($"An error occurred. Please check your internet connection and try again - {e.Message}", "");
			}
			string content = await message.Content.ReadAsStringAsync();
			if(!message.IsSuccessStatusCode) {
				return ($"Error - {content}", "");
			}
			//Otherwise, it was a sucess and we now have a guid representing our session.
			return ("", content);
		}
	}
}
