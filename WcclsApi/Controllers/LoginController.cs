using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WcclsCore;
using WcclsCore.Models.Request;

namespace WcclsApi.Controllers {

	[ApiController]
	[Route("[controller]")]
	public class LoginController : ControllerBase {

		public LoginController() {
			
		}

		///<summary>Logs the user in and returns a session id for the user.</summary>
		///<returns>The Session ID for this login. This will be used to successfully send every request.</returns>
		[HttpGet]
		public async Task<ObjectResult> Login([FromQuery]LoginRequest request) {
			if(string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Password)) {
				return new BadRequestObjectResult("Invalid Username or Password.");
			}
			WcclsWebScraping scraping = new WcclsWebScraping();
			string token = await scraping.Login("23614015566541", "1708");
			if(string.IsNullOrWhiteSpace(token)) {
				return BadRequest("Invalid Username or Password.");
			}
			return Ok(token);
		}
	}

}
