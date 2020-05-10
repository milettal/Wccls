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

		///<summary>Saves the user account to secure storage. This user account should already be authenticated as valid. Handles eventing.</summary>
		Task SaveUserAccount(User user);

		///<summary>Returns all currently logged in users.</summary>
		List<User> GetLoggedInUsers();

		///<summary>Removes the pass in user from the saved list of accounts. Handles eventing.</summary>
		Task RemoveUser(User user);

		Task UpdateNickname(string nicknameNew, User user);

	}
}
