using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WcclsMobile.Models;

namespace WcclsMobile.Services {
	///<summary>Handles all the ways to interact with user authentication. </summary>
	public interface IUserAuthenticationService {

		///<summary>Indicates that this user at least one account logged in.</summary>
		bool HasUserAccounts { get; }

		///<summary>Intializes this service and pulls any cached credentials available.</summary>
		Task InitializeService();

		///<summary>Attempts to login with the given username and password and adds it to the stored credentials.
		///Returns an error if something goes wrong.</summary>
		Task<string> AddUserAccount(string username, string password);

		///<summary>Returns all currently logged in users.</summary>
		List<User> GetLoggedInUsers();

	}
}
