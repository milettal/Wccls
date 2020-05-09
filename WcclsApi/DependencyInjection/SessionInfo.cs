using System.Net;
using System.Net.Http;

namespace WcclsApi.DependencyInjection {
	public class SessionInfo : ISessionInfo {
		public CookieContainer Session { get; set; }

		public HttpClient Client { get; set; }

		public long UserId { get; set; }

		public SessionInfo(HttpClient client, CookieContainer container, long userId) {
			Client = client;
			Session = container;
			UserId = userId;
		}
	}
}
