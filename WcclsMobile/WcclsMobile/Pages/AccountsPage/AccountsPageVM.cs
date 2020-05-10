using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using WcclsMobile.Events;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class AccountsPageVM : ViewModelBase {

		private IWcclsApiService _apiService { get; }

		private IUserAuthenticationService _userService { get; }

		private IEventAggregator _eventAggregator { get; }

		///<summary>A list of all currently logged in users.</summary>
		public ObservableCollection<User> ListUsers {
			get { return GetBindableProperty(() => ListUsers); }
			set { SetBindableProperty(() => ListUsers, value); }
		}

		public AccountsPageVM(INavigationService navigationService, IWcclsApiService apiService, IUserAuthenticationService userService, IEventAggregator aggregator)
			: base(navigationService)
		{
			_apiService = apiService;
			_userService = userService;
			_eventAggregator = aggregator;
			_eventAggregator
				.GetEvent<AccountsChangedEvent>()
				.Subscribe(() => { LoadUsers(); }, ThreadOption.UIThread);
			LoadUsers();
		}

		public override async Task NavigationTo(INavigationParameters parameters) {
			if(parameters == null) {
				return;
			}
			if(parameters.ContainsKey(AddUserVM.NEWUSER_KEY)) {
				User user = parameters.GetValue<User>(AddUserVM.NEWUSER_KEY);
				await _userService.SaveUserAccount(user);
				//Let all other views know that the number of accounts have been changed.
				//This will also refresh this views list.
				_eventAggregator.GetEvent<AccountsChangedEvent>().Publish();
			}
		}

		///<summary>Command for attempting to add a new user.</summary>
		public IAsyncCommand AddAccountCommand => GetCommandAsync(() => AddAccountCommand, true, async () => {
			await _navigationService.NavigateAsync(nameof(AddUser));
		});

		///<summary>Loads the users from the user service.</summary>
		private void LoadUsers() {
			ListUsers = _userService
				.GetLoggedInUsers()
				.OrderBy(x => x.Nickname)
				.ThenBy(x => x.Username)
				.ToObservableCollection();
		}
	}

}
