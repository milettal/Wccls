using System;
namespace Core.Wccls.Models.Result {
	public class LoginResult {
		///<summary>The username of this user.</summary>
		public string Username { get; set; }
		///<summary>The session id.</summary>
		public string SessionId { get; set; }
		///<summary>The url for payments.</summary>
		public string PaymentUrl { get; set; }
	}
}
