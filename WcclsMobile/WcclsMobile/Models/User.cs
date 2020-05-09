using System;
namespace WcclsMobile.Models {
	///<summary>Represents a single logged in user account.</summary>
	public class User {

		///<summary>The username for this account.</summary>
		public string Username { get; set; }

		///<summary>A nickname for this account. Can be changed.</summary>
		public string Nickname { get; set; }

		///<summary>The password for the user. Stored securely in keychain or whatever secure storage the platform supplies.</summary>
		public string Password { get; set; }

		///<summary>The current session id for this user. May be blank if freshly logged in. May be stale.</summary>
		public string SessionId { get; set; }
	}
}
