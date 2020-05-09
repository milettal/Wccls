﻿using System;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class FinesPageVM : ViewModelBase {

		private IUserAuthenticationService _userService { get; }

		///<summary>Indicates if the user is logged into at least one account.</summary>
		public bool IsUserLoggedIn => _userService?.HasUserAccounts??false;

		public FinesPageVM(INavigationService navigationService, IUserAuthenticationService userService) : base(navigationService) {
			_userService = userService;
		}



	}

}