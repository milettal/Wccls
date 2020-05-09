using System;
using System.Threading.Tasks;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using Prism.Services;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {
	public class AddUserVM : ViewModelBase {

		private const string USERNAME_KEY = "USERNAME_KEY";
		private const string PASSWORD_KEY = "PASSWORD_KEY";
		private const string SESSION_KEY = "SESSION_KEY";

		private IWcclsApiService _apiService { get; }

		private IPageDialogService _pageDialogService { get; }

		///<summary>Indicates if we are currently making a web call for logging in.</summary>
		public bool IsLoggingIn {
			get { return GetBindableProperty(() => IsLoggingIn, false); }
			set { SetBindableProperty(() => IsLoggingIn, value); }
		}

		///<summary>The username.</summary>
		public string Username {
			get { return GetBindableProperty(() => Username, ""); }
			set { SetBindableProperty(() => Username, value); AddUserCommand.ExecuteChanged(); }
		}

		///<summary>The password in plain text.</summary>
		public string Password {
			get { return GetBindableProperty(() => Password, ""); }
			set { SetBindableProperty(() => Password, value); AddUserCommand.ExecuteChanged(); }
		}

		public AddUserVM(INavigationService navigationService, IWcclsApiService apiService, IPageDialogService pageDialogService) : base(navigationService) {
			_apiService = apiService;
			_pageDialogService = pageDialogService;
		}

		///<summary>Attempts to add the user by confirming the username and password.</summary>
		public IAsyncCommand AddUserCommand => GetCommandAsync(() => AddUserCommand, true, async () => {
			if(string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) {
				return;
			}
			(string error, string sessionId) = await _apiService.Login(Username,Password);
			if(!string.IsNullOrWhiteSpace(error)) {
				await _pageDialogService.DisplayAlertAsync("Error", error, "Ok");
				return;
			}
			INavigationParameters paramaters = new NavigationParameters();
			paramaters.Add(USERNAME_KEY, Username);
			paramaters.Add(PASSWORD_KEY, Password);
			paramaters.Add(SESSION_KEY, sessionId);
			await _navigationService.ClearPopupStackAsync(paramaters);
		},() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));

		///<summary>Attempts to add the user by confirming the username and password.</summary>
		public IAsyncCommand CancelCommand => GetCommandAsync(() => CancelCommand, true, async () => {
			await _navigationService.ClearPopupStackAsync();
		});


	}
}
