using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Extensions;
using Prism.Events;
using WcclsMobile.Events;
using WcclsMobile.Models;

namespace WcclsMobile.Services {

	public class UserAuthenticationService : IUserAuthenticationService {

		private IEventAggregator _eventAggregator { get; }

		///<summary>An in memory cache of all the users with their sessions.</summary>
		private List<User> _listUsers { get; } = new List<User>();

		public bool HasUserAccounts => !_listUsers.IsNullOrEmpty();

		public UserAuthenticationService(IEventAggregator eventAggregator) {
			_eventAggregator = eventAggregator;
		}

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
			_eventAggregator.GetEvent<AccountsChangedEvent>().Publish();
			return Task.CompletedTask;
		}

		public Task RemoveUser(User user) {
			lock(_listUsers) {
				_listUsers.RemoveAll(x => x.Username == user.Username);
			}
			_eventAggregator.GetEvent<AccountsChangedEvent>().Publish();
			return Task.CompletedTask;
		}

		public Task UpdateNickname(string nicknameNew, User user) {
			lock(_listUsers) {
				User userMatching = _listUsers.Where(x => x.Username == user.Username).FirstOrDefault();
				if(userMatching != null) {
					userMatching.Nickname = nicknameNew;
				}
			}
			_eventAggregator.GetEvent<AccountsChangedEvent>().Publish();
			return Task.CompletedTask;
		}
	}
}
