using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using WcclsCore.Models.Result;
using WcclsMobile.Events;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class FinesPageVM : ViewModelBase {

		private IUserAuthenticationService _userService { get; }

		private IEventAggregator _eventAggregator { get; }

		private IWcclsApiService _apiService { get; }

		///<summary>A list of currently logged in users. If we get an event, we can tell if this module needs to be refreshed or not.</summary>
		private List<User> _listUsersCur { get; set; }

		///<summary>Indicates if the user is logged into at least one account.</summary>
		public bool IsUserLoggedIn => _userService?.HasUserAccounts??false;

		///<summary>Indicates if we are currently loading the fines for these accounts/users.</summary>
		public bool IsLoadingFines {
			get { return GetBindableProperty(() => IsLoadingFines,false); }
			set { SetBindableProperty(() => IsLoadingFines, value); }
		}

		///<summary>A list of all users and their fines.</summary>
		public List<UserFines> ListUserFines {
			get { return GetBindableProperty(() => ListUserFines); }
			set { SetBindableProperty(() => ListUserFines, value); }
		}

		///<summary>Any errors that occured from getting the fines.</summary>
		public string Errors {
			get { return GetBindableProperty(() => Errors, ""); }
			set { SetBindableProperty(() => Errors, value); }
		}

		///<summary>The selected user fine. Only for turning off the selection.</summary>
		public UserFines SelectedUserFines {
			get { return null; }
			set { RaisePropertyChanged(() => SelectedUserFines); }
		}

		///<summary>The url for making payments.</summary>
		public string PaymentUrl {
			get { return GetBindableProperty(() => PaymentUrl, ""); }
			set { SetBindableProperty(() => PaymentUrl, value); }
		}

		public FinesPageVM(INavigationService navigationService, IUserAuthenticationService userService, IEventAggregator eventAggregator, IWcclsApiService apiService)
			: base(navigationService)
		{
			_userService = userService;
			_eventAggregator = eventAggregator;
			_apiService = apiService;
			_eventAggregator.GetEvent<AccountsChangedEvent>().Subscribe(() => {
				int countOld = _listUsersCur.Count;
				_listUsersCur = _userService
					.GetLoggedInUsers()
					.OrderBy(x => x.Nickname)
					.ToList();
				//They changed the number of users.
				if(countOld != _listUsersCur.Count) {
					//They may have gone from having no users to one or more or visa versa
					RaisePropertyChanged(() => IsUserLoggedIn);
					TaskUtils.FireAndForget(RefreshFines);
				}
			}, ThreadOption.UIThread);
			_listUsersCur = _userService
				.GetLoggedInUsers()
				.OrderBy(x => x.Nickname)
				.ToList();
		}

		public override async Task InitializeVMAsync(INavigationParameters parameters) {
			await RefreshFines();
		}

		public IAsyncCommand<UserFines> UserFinesSelectedCommand => GetCommandAsync(() => UserFinesSelectedCommand, true, async (userFines) => {
			if(userFines == null) {
				return;
			}
			await _navigationService.NavigateAsync(nameof(FinesDetailsPage), (FinesDetailsPageVM.USER_FINES_KEY, userFines));
		});

		public IAsyncCommand RefreshCommand => GetCommandAsync(() => RefreshCommand, true, async () => {
			await RefreshFines();
		});

		///<summary>Refreshes all of the fines for all users.</summary>
		private async Task RefreshFines() {
			if(IsLoadingFines) {
				return;
			}
			Errors = "";
			if(!IsUserLoggedIn) {
				return;
			}
			PaymentUrl = _apiService.GetPaymentUrl();
			IsLoadingFines = true;
			List<UserFines> listUserFines = new List<UserFines>();
			StringBuilder strBuilder = new StringBuilder();
			await TaskUtils.RunInParallel(_listUsersCur, async (user) => {
				(string error, FinesResult result) = await _apiService.Fines(user);
				if(!string.IsNullOrEmpty(error)) {
					strBuilder.AppendLine(error);
				}
				else {
					listUserFines.Add(new UserFines {
						User = user,
						Fines = result,
						PaymentUrl = PaymentUrl,
					});
				}
			});
			Errors = strBuilder.ToString();
			ListUserFines = listUserFines
				.OrderBy(x => x.User.Nickname)
				.ToList();
			IsLoadingFines = false;
		}

	}

}
