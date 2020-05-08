using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Core {
	public static class HttpUtils {

		///<summary>Extracts all "Set-Cookie" headers from a given message.</summary>
		///<param name="message">The response message from an http request.</param>
		///<returns>A list of key value pairs for "Set-Cookie" headers from an http response.</returns>
		public static List<Cookie> GetSetCookieParams(HttpResponseMessage message) {
			message = message ?? throw new ArgumentNullException(nameof(message));
			message.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookies);
			CookieContainer container = new CookieContainer();
			foreach(string cookie in cookies) {
				container.SetCookies(message.RequestMessage.RequestUri, cookie);
			}
			List<Cookie> retVal = new List<Cookie>();
			foreach(Cookie cook in container.GetCookies(message.RequestMessage.RequestUri)) {
				retVal.Add(cook);
			}
			return retVal;
		}
	}
}
