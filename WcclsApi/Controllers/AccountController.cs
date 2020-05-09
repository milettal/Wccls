using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WcclsApi.DependencyInjection;
using WcclsCore;
using WcclsCore.Models;
using WcclsCore.Models.Request;
using WcclsCore.Models.Result;

namespace WcclsApi.Controllers {

	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class AccountController : ControllerBase {

		private const string PAYMENT_URL = "https://1811.ecs.envisionware.net/eCommerceWebModule/Home";

		private ISessionInfo _sessionInfo { get; }

		public AccountController(ISessionInfo sessionInfo) {
			_sessionInfo = sessionInfo;
		}

		///<summary>Logs the user in and returns a session id for the user.</summary>
		///<returns>The Session ID for this login. This will be used to successfully send every request.</returns>
		[HttpGet]
		[Route("borrow")]
		public async Task<ObjectResult> Borrowing([FromQuery]string sessionId) {
			if(string.IsNullOrWhiteSpace(sessionId)) {
				return new BadRequestObjectResult("Invalid Session Id.");
			}
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client);
			Borrowing retVal;
			try {
				retVal = await scraping.GetBorrowing();
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(retVal);
		}

		///<summary>Returns all fines for this account.</summary>
		[HttpGet]
		[Route("fines")]
		public async Task<ObjectResult> GetFines() {
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client);
			FinesResult result;
			try {
				result = await scraping.GetFines(_sessionInfo.UserId);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(result);
		}

		[HttpGet]
		[Route("holds")]
		public async Task<ObjectResult> GetHolds() {
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client);
			HoldsResult result;
			try {
				result = await scraping.GetHolds(_sessionInfo.UserId);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(result);
		}

		[HttpGet]
		[Route("checkout")]
		public async Task<ObjectResult> GetCheckedOut() {
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client);
			CheckedOutResult result;
			try {
				result=await scraping.GetCheckedOut(_sessionInfo.UserId);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(result);
		}

		[HttpPost]
		[Route("renew")]
		public async Task<ObjectResult> RenewItem([FromBody] RenewItemsRequest request) {
			if(request?.ListCheckoutIds == null || request.ListCheckoutIds.Count == 0) {
				return new BadRequestObjectResult("Checkout IDs required");
			}
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client);
			RenewItemsResult result = await scraping.RenewItems(request.ListCheckoutIds, _sessionInfo.UserId);
			return Ok(result);
		}

		///<summary>Returns the URL for making payments. This is an API call in case the URL is changed.</summary>
		[HttpGet]
		[Route("payment/url")]
		public string GetPaymentUrl() {
			return PAYMENT_URL;
		}
	}
}
