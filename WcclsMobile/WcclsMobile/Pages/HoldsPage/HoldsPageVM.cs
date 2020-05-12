using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using WcclsCore.Models.Result;
using WcclsMobile.Events;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class HoldsPageVM : ViewModelBase {

		private IUserAuthenticationService _userService { get; }

		private IEventAggregator _eventAggregator { get; }

		private IWcclsApiService _apiService { get; }

		///<summary>A list of users as this VM is aware of them.</summary>
		private List<User> _listUsersCur { get; set; }

		///<summary>Indicates if the user is logged into at least one account.</summary>
		public bool IsUserLoggedIn => _userService?.HasUserAccounts??false;

		///<summary>Indicates if we are currently loading the holds for these accounts/users.</summary>
		public bool IsLoadingHolds {
			get { return GetBindableProperty(() => IsLoadingHolds, false); }
			set { SetBindableProperty(() => IsLoadingHolds, value); }
		}

		///<summary>Any errors associated to getting t he holds for these accounts.</summary>
		public string Errors {
			get { return GetBindableProperty(() => Errors, ""); }
			set { SetBindableProperty(() => Errors, value); }
		}

		///<summary>The total number of active holds across all accounts.
		///These properties must match the name of the one in the HoldsResult.</summary>
		public int ActiveHolds {
			get { return GetBindableProperty(() => ActiveHolds); }
			set { SetBindableProperty(() => ActiveHolds, value); }
		}

		///<summary>The total number of inactive holds across all accounts.
		///These properties must match the name of the one in the HoldsResult.</summary>
		public int InactiveHolds {
			get { return GetBindableProperty(() => InactiveHolds); }
			set { SetBindableProperty(() => InactiveHolds, value); }
		}

		///<summary>The total number of holds ready for pickup across all accounts.
		///These properties must match the name of the one in the HoldsResult.</summary>
		public int ReadyForPickup {
			get { return GetBindableProperty(() => ReadyForPickup); }
			set { SetBindableProperty(() => ReadyForPickup, value); }
		}

		///<summary>Indicates if we are currently loading the holds for these accounts/users.</summary>
		public List<UserHolds> ListUserHolds {
			get { return GetBindableProperty(() => ListUserHolds); }
			set { SetBindableProperty(() => ListUserHolds, value); }
		}

		public UserHolds SelectedUserHolds {
			get { return null; }
			set { RaisePropertyChanged(() => SelectedUserHolds); }
		}

		public HoldsPageVM(INavigationService navigationService, IUserAuthenticationService userService, IEventAggregator eventAggregator, IWcclsApiService apiService)
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
					TaskUtils.FireAndForget(RefreshHolds);
				}
			}, ThreadOption.UIThread);
			_eventAggregator.GetEvent<RefreshHoldsEvent>().Subscribe(() => {
				TaskUtils.FireAndForget(RefreshHolds);
			}, ThreadOption.UIThread);
			_listUsersCur = _userService
				.GetLoggedInUsers()
				.OrderBy(x => x.Nickname)
				.ToList();
		}

		public override async Task InitializeVMAsync(INavigationParameters parameters) {
			TaskUtils.FireAndForget(RefreshHolds);
		}

		///<summary>Command called when users manually want to refresh</summary>
		public IAsyncCommand RefreshCommand => GetCommandAsync(() => RefreshCommand, true, async () => {
			await RefreshHolds();
		});

		public IAsyncCommand<UserHolds> UserHoldsSelectedCommand => GetCommandAsync(() => UserHoldsSelectedCommand, true, async (holds) => {
			if(holds == null) {
				return;
			}
			//Open the detail page.
		});

		private async Task RefreshHolds() {
			if(IsLoadingHolds) {
				return;
			}
			Errors = "";
			if(!IsUserLoggedIn) {
				return;
			}
			IsLoadingHolds = true;
			StringBuilder strBuilder = new StringBuilder();
			List<UserHolds> listHolds = new List<UserHolds>();
			await TaskUtils.RunInParallel(_listUsersCur, async (user) => {
				(string error, HoldsResult result) = await _apiService.Holds(user);
				if(!string.IsNullOrEmpty(error)) {
					strBuilder.AppendLine($"Error for user {user.Nickname}: {error}");
				}
				else {
					//Try and preserve the old instance. If the user has multiple accounts and has a detail page open,
					//we don't want that to contain stale data.
					UserHolds holdsMatching = ListUserHolds?.FirstOrDefault(x => x.User.Username == user.Username);
					if(holdsMatching == null) {
						listHolds.Add(new UserHolds(user, result, _navigationService));
					}
					else {
						holdsMatching.User = user;
						holdsMatching.Holds = result;
						listHolds.Add(holdsMatching);
					}
				}
			});
			ActiveHolds = listHolds.Sum(x => x.Holds.ActiveHolds);
			InactiveHolds = listHolds.Sum(x => x.Holds.InactiveHolds);
			ReadyForPickup = listHolds.Sum(x => x.Holds.ReadyForPickup);
			Errors = strBuilder.ToString();
			ListUserHolds = listHolds;
			IsLoadingHolds = false;
		}



	}

}
