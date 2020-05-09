using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace WcclsApi.DependencyInjection {
	public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions> {

		private ISessionCache _sessionCache { get; }

		private ISessionInfo _sessionInfo { get; }

		public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,ILoggerFactory logger, UrlEncoder urlEncoder, ISystemClock systemClock, ISessionCache sessionCache,
			ISessionInfo sessionInfo)
			: base(options,logger,urlEncoder,systemClock) 
		{
			_sessionCache = sessionCache;
			_sessionInfo = sessionInfo;
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
			if(!Request.Headers.TryGetValue("SessionGuid",out StringValues headerValues)) {
				return Task.FromResult(AuthenticateResult.Fail("Missing session headers."));
			}
			string guid = headerValues.ToArray().FirstOrDefault();
			(CookieContainer container, long userId) = _sessionCache.GetCachedSession(guid);
			if(container==null || userId <= 0) {
				return Task.FromResult(AuthenticateResult.Fail("Invalid GUID."));
			}
			_sessionInfo.UserId = userId;
			_sessionInfo.Session = container;
			_sessionInfo.Client = new HttpClient(new HttpClientHandler { CookieContainer = container });
			var identity = new ClaimsIdentity(new Claim[0], Scheme.Name);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);
			return Task.FromResult(AuthenticateResult.Success(ticket));
		}
	}
}
