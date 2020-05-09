using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WcclsMobile.Models;

namespace WcclsMobile.Services {

	public class UserAuthenticationService : IUserAuthenticationService {

		///<summary>An in memory cache of all the users with their sessions.</summary>
		private List<User> _listUsers { get; } = new List<User>();

		public bool HasUserAccounts {
			get {
				return false;
			}
		}

		public Task InitializeService() {
			throw new NotImplementedException();
		}

		public Task<string> AddUserAccount(string username, string password) {
			throw new NotImplementedException();
		}

		public List<User> GetLoggedInUsers() {
			lock(_listUsers) {
				//Returning a deep copy of the list, but a shallow copy of the users.
				//For now, I think this is fine.
				return new List<User>(_listUsers);
			}
		}
	}
}
