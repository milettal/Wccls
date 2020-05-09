using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

		private ISessionCache _sessionCache { get; }

		public LoginController(ISessionCache sessionCache) {
			_sessionCache = sessionCache;
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
			WcclsWebScraping scraping = new WcclsWebScraping(client);
			long userId = await scraping.Login(request.Username, request.Password);
			if(userId <= 0) {
				return BadRequest("Invalid Username or Password.");
			}
			string sessionGuid = _sessionCache.AddSession(container, userId);
			return Ok(sessionGuid);
		}
	}

}
