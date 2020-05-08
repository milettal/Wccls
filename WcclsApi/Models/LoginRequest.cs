using Microsoft.AspNetCore.Mvc;

namespace WcclsApi.Models {
	///<summary>The request model for the login method.</summary>
	public class LoginRequest {

		///<summary>The username to log in with.</summary>
		[FromQuery]
		public string Username { get; set; }

		///<summary>The password to log in with.</summary>
		[FromQuery]
		public string Password { get; set; }
	}
}
