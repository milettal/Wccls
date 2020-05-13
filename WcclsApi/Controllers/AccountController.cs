using System;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Wccls.Models.Request;
using Core.Wccls.Models.Result;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WcclsApi.DependencyInjection;
using WcclsCore.Models;
using WcclsCore.Models.Request;
using WcclsCore.Models.Result;

namespace WcclsApi.Controllers {

	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class AccountController : ControllerBase {

		private ISessionInfo _sessionInfo { get; }

		private ISystemClock _systemClock { get; }

		public AccountController(ISessionInfo sessionInfo, ISystemClock systemClock) {
			_sessionInfo = sessionInfo;
			_systemClock = systemClock;
		}

		///<summary>Logs the user in and returns a session id for the user.</summary>
		///<returns>The Session ID for this login. This will be used to successfully send every request.</returns>
		[HttpGet]
		[Route("borrow")]
		public async Task<ObjectResult> Borrowing([FromQuery]string sessionId) {
			if(string.IsNullOrWhiteSpace(sessionId)) {
				return new BadRequestObjectResult("Invalid Session Id.");
			}
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
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
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
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
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
			HoldsResult result;
			try {
				result = await scraping.GetHolds(_sessionInfo.UserId);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(result);
		}

		[HttpPost]
		[Route("holds/pauseholds")]
		public async Task<ObjectResult> PauseHolds([FromBody] PauseHoldsRequest request) {
			if(request == null) {
				return new BadRequestObjectResult("Null request.");
			}
			if(request.EndDate <= _systemClock.UtcNow.LocalDateTime) {
				return new BadRequestObjectResult("Bad EndDate");
			}
			if(request.ListHoldIds.IsNullOrEmpty()) {
				return new BadRequestObjectResult("No Hold Ids provided.");
			}
			PauseHoldsResult result;
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
			try {
				result = await scraping.PauseHolds(_sessionInfo.UserId, request.ListHoldIds, request.EndDate, request.IsCurrentlyActive);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(result);
		}

		[HttpPost]
		[Route("holds/resumeholds")]
		public async Task<ObjectResult> ResumeHolds([FromBody] ResumeHoldsRequest request) {
			if(request==null) {
				return new BadRequestObjectResult("Null request.");
			}
			if(request.ListHoldIds.IsNullOrEmpty()) {
				return new BadRequestObjectResult("No Hold Ids provided.");
			}
			ResumeHoldsResult result;
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
			try {
				result = await scraping.ResumeHolds(_sessionInfo.UserId, request.ListHoldIds);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok(result);
		}

		[HttpPost]
		[Route("holds/cancelholds")]
		public async Task<ObjectResult> CancelHolds([FromBody] CancelHoldsRequest request) {
			if(request == null) {
				return new BadRequestObjectResult("Null request.");
			}
			if(request.ListHoldIds.IsNullOrEmpty()) {
				return new BadRequestObjectResult("No Hold Ids provided.");
			}
			if(request.ListMetadataIds.IsNullOrEmpty()) {
				return new BadRequestObjectResult("No metadataids provided");
			}
			if(request.ListHoldIds.Count != request.ListMetadataIds.Count) {
				return new BadRequestObjectResult("A different number of hold ids than metadata ids.");
			}
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
			try {
				await scraping.CancelHolds(_sessionInfo.UserId, request.ListHoldIds, request.ListMetadataIds);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok("Success");
		}

		[HttpPost]
		[Route("holds/changeholdslocation")]
		public async Task<ObjectResult> ChangeHoldsLocation([FromBody] ChangeHoldsLocationRequest request) {
			if(request == null) {
				return new BadRequestObjectResult("Null request.");
			}
			if(request.ListHoldIds.IsNullOrEmpty()) {
				return new BadRequestObjectResult("No Hold Ids provided.");
			}
			if(request.NewLocation == Library.Unknown || request.NewLocation == Library.WCCLSCourier) {
				return new BadRequestObjectResult($"Invalid library: {request.NewLocation.ToString()}");
			}
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
			try {
				await scraping.ChangeHoldsLocation(_sessionInfo.UserId, request.ListHoldIds, request.NewLocation);
			}
			catch(Exception e) {
				return new BadRequestObjectResult("bad");
			}
			return Ok("Success");
		}

		[HttpGet]
		[Route("checkout")]
		public async Task<ObjectResult> GetCheckedOut() {
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
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
			WcclsWebScraping scraping = new WcclsWebScraping(_sessionInfo.Client, _systemClock);
			RenewItemsResult result = await scraping.RenewItems(request.ListCheckoutIds, _sessionInfo.UserId);
			return Ok(result);
		}
	}
}
