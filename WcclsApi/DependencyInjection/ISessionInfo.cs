using System.Net;
using System.Net.Http;

namespace WcclsApi.DependencyInjection {
	public interface ISessionInfo {
		///<summary>The cookies for this session.</summary>
		CookieContainer Session { get; set; }
		///<summary>The client for this session.</summary>
		HttpClient Client { get; set; }
		///<summary>The user id for this session.</summary>
		long UserId { get; set; }
	}
}
