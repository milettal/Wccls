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

		///<summary>The currently selected user. Used to programatically lose retention after the initial click.</summary>
		public User SelectedUser {
			get { return null; }
			set { RaisePropertyChanged(() => SelectedUser); }
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
				//This call will handle eventing which will refresh this view and others.
				await _userService.SaveUserAccount(user);
			}
		}

		///<summary>Command for attempting to add a new user.</summary>
		public IAsyncCommand AddAccountCommand => GetCommandAsync(() => AddAccountCommand, true, async () => {
			await _navigationService.NavigateAsync(nameof(AddUser));
		});

		///<summary>Command that occurs when a user is selected. This should take the user to the account detail page.</summary>
		public IAsyncCommand<User> UserSelectedCommand => GetCommandAsync(() => UserSelectedCommand, true, async (user) => {
			if(user == null) {
				return;
			}
			await _navigationService.NavigateAsync(nameof(AccountDetailPage), (AccountDetailPageVM.USER_KEY, user));
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
