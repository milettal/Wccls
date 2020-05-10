using System;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using WcclsMobile.Events;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class CheckedOutPageVM : ViewModelBase {

		private IUserAuthenticationService _userService { get; }

		private IEventAggregator _eventAggregator { get; }

		///<summary>Indicates if the user is logged into at least one account.</summary>
		public bool IsUserLoggedIn => _userService?.HasUserAccounts??false;

		public CheckedOutPageVM(INavigationService navigationService, IUserAuthenticationService userService, IEventAggregator aggregator) : base(navigationService) {
			_userService = userService;
			_eventAggregator = aggregator;
			_eventAggregator.GetEvent<AccountsChangedEvent>().Subscribe(() => {
				//They may have gone from having no users to one or more or visa versa
				RaisePropertyChanged(() => IsUserLoggedIn);
			}, ThreadOption.UIThread);
		}

	}

}
