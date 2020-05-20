using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using WcclsCore.Models;
using WcclsCore.Models.Result;
using WcclsMobile.Events;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class CheckedOutPageVM : ViewModelBase {

		private IUserAuthenticationService _userService { get; }

		private IEventAggregator _eventAggregator { get; }

		private IWcclsApiService _apiService { get; }

		///<summary>A list of users as this VM is aware of them.</summary>
		private List<User> _listUsersCur { get; set; }

		///<summary>Indicates if the user is logged into at least one account.</summary>
		public bool IsUserLoggedIn => _userService?.HasUserAccounts??false;

		///<summary>Indicates if we are currently loading the checked out items for these accounts/users.</summary>
		public bool IsLoadingCheckOuts {
			get { return GetBindableProperty(() => IsLoadingCheckOuts, false); }
			set { SetBindableProperty(() => IsLoadingCheckOuts, value); }
		}

		///<summary>Any errors associated to getting the checked out items for these accounts.</summary>
		public string Errors {
			get { return GetBindableProperty(() => Errors, ""); }
			set { SetBindableProperty(() => Errors, value); }
		}

		///<summary>The current list of users and their associated check outs.</summary>
		public List<UserCheckOuts> ListUserCheckOuts {
			get { return GetBindableProperty(() => ListUserCheckOuts); }
			set { SetBindableProperty(() => ListUserCheckOuts, value); }
		}

		public CheckedOutSummary CheckedOutSummary {
			get { return GetBindableProperty(() => CheckedOutSummary); }
			set { SetBindableProperty(() => CheckedOutSummary, value); }
		}

		public UserCheckOuts SelectedUserCheckOuts {
			get { return null; }
			set { RaisePropertyChanged(() => SelectedUserCheckOuts); }
		}


		public CheckedOutPageVM(INavigationService navigationService, IUserAuthenticationService userService, IEventAggregator aggregator, IWcclsApiService apiService)
			: base(navigationService)
		{
			_userService = userService;
			_eventAggregator = aggregator;
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
					TaskUtils.FireAndForget(RefreshCheckOuts);
				}
			}, ThreadOption.UIThread);
			_listUsersCur = _userService
				.GetLoggedInUsers()
				.OrderBy(x => x.Nickname)
				.ToList();
		}

		public override async Task InitializeVMAsync(INavigationParameters parameters) {
			TaskUtils.FireAndForget(RefreshCheckOuts);
		}

		///<summary>Command called when users manually want to refresh.</summary>
		public IAsyncCommand RefreshCommand => GetCommandAsync(() => RefreshCommand, true, async () => {
			await RefreshCheckOuts();
		});

		public IAsyncCommand<UserCheckOuts> UserCheckOutsSelectedCommand => GetCommandAsync(() => UserCheckOutsSelectedCommand, true, async (checkOuts) => {
			if(checkOuts == null) {
				return;
			}
			//Open the detail page.
		});

		private async Task RefreshCheckOuts() {
			if(IsLoadingCheckOuts) {
				return;
			}
			Errors = "";
			if(!IsUserLoggedIn) {
				return;
			}
			IsLoadingCheckOuts = true;
			StringBuilder strBuilder = new StringBuilder();
			List<UserCheckOuts> listCheckOuts = new List<UserCheckOuts>();
			await TaskUtils.RunInParallel(_listUsersCur, async (user) => {
				(string error, CheckedOutResult result) = await _apiService.CheckedOut(user);
				if(!string.IsNullOrEmpty(error)) {
					strBuilder.AppendLine($"Error for user {user.Nickname}: {error}");
				}
				else {
					//Always order by the due date. Order by ID after so that the order is deterministic
					result.ListCheckedOutItems = result
						.ListCheckedOutItems
						.OrderBy(x => x.DueDate)
						.ThenBy(x => x.Id)
						.ToList();
					//Try and preserve the old instance. If the user has multiple accounts and has a detail page open,
					//we don't want that to contain stale data.
					UserCheckOuts checkOutMatching = ListUserCheckOuts?.FirstOrDefault(x => x.User.Username==user.Username);
					if(checkOutMatching == null) {
						listCheckOuts.Add(new UserCheckOuts(user, result, _navigationService));
					}
					else {
						checkOutMatching.User = user;
						checkOutMatching.CheckedOut = result;
						listCheckOuts.Add(checkOutMatching);
					}
				}
			});
			CheckedOutSummary = new CheckedOutSummary {
				TotalItems = listCheckOuts.Sum(x => x.CheckedOut.TotalCheckedOut),
				OutItems = listCheckOuts.Sum(x => x.CheckedOut.ListCheckedOutItems.Count(y => y.Status==CheckoutStatus.Out)),
				DueSoonItems = listCheckOuts.Sum(x => x.CheckedOut.ListCheckedOutItems.Count(y => y.Status==CheckoutStatus.Coming_due)),
				OverdueItems = listCheckOuts.Sum(x => x.CheckedOut.ListCheckedOutItems.Count(y => y.Status==CheckoutStatus.Overdue)),
				NextDue = listCheckOuts.Min(x => x.CheckedOut.NextDueDate),
			};
			Errors = strBuilder.ToString();
			ListUserCheckOuts = listCheckOuts
				.OrderBy(x => x.User.Nickname)
				.ToList();
			IsLoadingCheckOuts = false;
		}

	}

}
