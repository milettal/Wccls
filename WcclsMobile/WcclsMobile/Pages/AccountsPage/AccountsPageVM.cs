using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Extensions;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class AccountsPageVM : ViewModelBase {

		private IWcclsApiService _apiService { get; }

		private IUserAuthenticationService _userService { get; }

		///<summary>A list of all currently logged in users.</summary>
		public ObservableCollection<User> ListUsers {
			get { return GetBindableProperty(() => ListUsers); }
			set { SetBindableProperty(() => ListUsers, value); }
		}

		public AccountsPageVM(INavigationService navigationService, IWcclsApiService apiService, IUserAuthenticationService userService) : base(navigationService) {
			_apiService = apiService;
			_userService = userService;
			ListUsers = _userService
				.GetLoggedInUsers()
				.OrderBy(x => x.Nickname)
				.ThenBy(x => x.Username)
				.ToObservableCollection();
		}

		///<summary>Command for attempting to add a new user.</summary>
		public IAsyncCommand AddAccountCommand => GetCommandAsync(() => AddAccountCommand, true, async () => {
			await _navigationService.NavigateAsync(nameof(AddUser));
		});

	}

}
