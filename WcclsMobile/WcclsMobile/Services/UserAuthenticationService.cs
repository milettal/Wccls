using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Extensions;
using WcclsMobile.Models;

namespace WcclsMobile.Services {

	public class UserAuthenticationService : IUserAuthenticationService {

		///<summary>An in memory cache of all the users with their sessions.</summary>
		private List<User> _listUsers { get; } = new List<User>();

		public bool HasUserAccounts => !_listUsers.IsNullOrEmpty();

		public Task InitializeService() {
			throw new NotImplementedException();
		}

		public List<User> GetLoggedInUsers() {
			lock(_listUsers) {
				//Returning a deep copy of the list, but a shallow copy of the users.
				//For now, I think this is fine.
				return new List<User>(_listUsers);
			}
		}

		public Task SaveUserAccount(User user) {
			lock(_listUsers) {
				if(_listUsers.Any(x => x.Username == user.Username)) {
					throw new ApplicationException($"This user already exists in the {nameof(UserAuthenticationService)}.");
				}
				_listUsers.Add(user);
			}
			return Task.CompletedTask;
		}
	}
}
