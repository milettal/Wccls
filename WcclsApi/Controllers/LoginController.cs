using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Wccls.Models.Result;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WcclsApi.DependencyInjection;
using WcclsCore;
using WcclsCore.Models.Request;

namespace WcclsApi.Controllers {

	[ApiController]
	[Route("[controller]")]
	[AllowAnonymous]
	public class LoginController : ControllerBase {

		private const string PAYMENT_URL = "https://1811.ecs.envisionware.net/eCommerceWebModule/Home";

		private ISessionCache _sessionCache { get; }

		private ISystemClock _systemClock { get; }

		public LoginController(ISessionCache sessionCache, ISystemClock systemClock) {
			_sessionCache = sessionCache;
			_systemClock = systemClock;
		}

		///<summary>Logs the user in and returns a session id for the user.</summary>
		///<returns>The Session ID for this login. This will be used to successfully send every request.</returns>
		[HttpGet]
		public async Task<ObjectResult> Login([FromQuery]LoginRequest request) {
			if(string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Password)) {
				return new BadRequestObjectResult("Invalid Username or Password.");
			}
			CookieContainer container = new CookieContainer();
			using HttpClient client = new HttpClient(new HttpClientHandler { CookieContainer = container });
			WcclsWebScraping scraping = new WcclsWebScraping(client, _systemClock);
			(long userId, string username) = await scraping.Login(request.Username, request.Password);
			if(userId <= 0) {
				return BadRequest("Invalid Username or Password.");
			}
			string sessionGuid = _sessionCache.AddSession(container, userId);
			return Ok(new LoginResult { Username = username, SessionId = sessionGuid, PaymentUrl = PAYMENT_URL });
		}
	}

}
