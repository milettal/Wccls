using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WcclsApi.Models;
using WcclsCore;

namespace WcclsApi.Controllers {

	[ApiController]
	[Route("[controller]")]
	public class AccountController : ControllerBase {

		public AccountController() {

		}

		///<summary>Logs the user in and returns a session id for the user.</summary>
		///<returns>The Session ID for this login. This will be used to successfully send every request.</returns>
		[HttpGet]
		public async Task<ObjectResult> Borrowing([FromQuery]string sessionId) {
			if(string.IsNullOrWhiteSpace(sessionId)) {
				return new BadRequestObjectResult("Invalid Session Id.");
			}
			WcclsWebScraping scraping = new WcclsWebScraping();
			Borrowing retVal;
			try {
				retVal = await scraping.GetBorrowing();
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(retVal);
		}
	}
}
