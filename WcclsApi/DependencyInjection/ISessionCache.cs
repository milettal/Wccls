using System.Net;

namespace WcclsApi.DependencyInjection {
	public interface ISessionCache {
		///<summary>Returns the unique session for the given guid as well as the user id. If the session is older than a certain time frame or there never was a session, this
		///will return null.</summary>
		(CookieContainer container, long userId) GetCachedSession(string guid);
		///<summary>Adds the given container as a session and returns the unique identifier for the session.</summary>
		string AddSession(CookieContainer container, long userId);
	}
}
