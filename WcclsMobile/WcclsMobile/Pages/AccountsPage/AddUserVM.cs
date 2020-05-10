using System.Linq;
using System.Threading;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {
	public class AddUserVM : ViewModelBase {

		public const string NEWUSER_KEY = "NEWUSER_KEY";

		private IWcclsApiService _apiService { get; }

		private IPageDialogService _pageDialogService { get; }

		private IUserAuthenticationService _userService { get; }

		///<summary>A token to be used when the user cancels from this dialog, specifically with a query/web call in progress.</summary>
		private CancellationTokenSource _canellationToken { get; } = new CancellationTokenSource();

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

		public AddUserVM(INavigationService navigationService, IWcclsApiService apiService, IPageDialogService pageDialogService, IUserAuthenticationService userService)
			: base(navigationService)
		{
			_apiService = apiService;
			_pageDialogService = pageDialogService;
			_userService = userService;
		}

		///<summary>Attempts to add the user by confirming the username and password.</summary>
		public IAsyncCommand AddUserCommand => GetCommandAsync(() => AddUserCommand, true, async () => {
			if(string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) {
				return;
			}
			if(_userService.GetLoggedInUsers().Any(x => x.Username == Username)) {
				await _pageDialogService.DisplayAlertAsync("Error", "A user with this username/card number is already logged in.", "Ok");
				return;
			}
			IsLoggingIn = true;
			(string error, string sessionId) = await _apiService.Login(Username,Password);
			IsLoggingIn = false;
			//We were cancelled. Don't show or do anything.
			if(_canellationToken.IsCancellationRequested) {
				return;
			}
			if(!string.IsNullOrWhiteSpace(error)) {
				await _pageDialogService.DisplayAlertAsync("Error", error, "Ok");
				return;
			}
			await _navigationService.ClearPopupStackAsync(NEWUSER_KEY, new User {
				Nickname = Username,
				Username = Username,
				Password = Password,
				SessionId = sessionId,
			});
		},() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));

		///<summary>Attempts to add the user by confirming the username and password.</summary>
		public IAsyncCommand CancelCommand => GetCommandAsync(() => CancelCommand, true, async () => {
			_canellationToken.Cancel();
			await _navigationService.ClearPopupStackAsync();
		});


	}
}
