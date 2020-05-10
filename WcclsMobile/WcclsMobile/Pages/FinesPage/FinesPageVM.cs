using System;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using WcclsMobile.Events;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class FinesPageVM : ViewModelBase {

		private IUserAuthenticationService _userService { get; }

		private IEventAggregator _eventAggregator { get; }

		///<summary>Indicates if the user is logged into at least one account.</summary>
		public bool IsUserLoggedIn => _userService?.HasUserAccounts??false;

		public FinesPageVM(INavigationService navigationService, IUserAuthenticationService userService, IEventAggregator eventAggregator) : base(navigationService) {
			_userService = userService;
			_eventAggregator=eventAggregator;
			_eventAggregator.GetEvent<AccountsChangedEvent>().Subscribe(() => {
				//They may have gone from having no users to one or more or visa versa
				RaisePropertyChanged(() => IsUserLoggedIn);
			}, ThreadOption.UIThread);
		}



	}

}
