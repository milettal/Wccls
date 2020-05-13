using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.DependencyInjection;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using WcclsCore.Models;
using WcclsMobile.Events;
using WcclsMobile.Models;
using WcclsMobile.Services;

namespace WcclsMobile.Pages {

	public class HoldOptionsPopupPageVM : ViewModelBase {

		public const string HOLD_KEY = "HOLD_KEY";
		public const string USER_KEY = "USER_KEY";

		private IPageDialogService _pageDialogService { get; }

		private IEventAggregator _eventAggregator { get; }

		private IClock _clock { get; }

		private IWcclsApiService _wcclsApi { get; }

		private User _user { get; set; }

		///<summary>Indicates if the user can cancel this hold.</summary>
		public bool CanUserCancelHold => Hold?.ListActions?.Contains(ItemAction.Cancel) ?? false;

		///<summary>Indicates if the user can suspend/pause this hold.</summary>
		public bool CanUserSuspendHold => Hold?.ListActions?.Contains(ItemAction.Suspend) ?? false;

		///<summary>Indicates if the user can change the pickup location for this hold.</summary>
		public bool CanUserChangePickupLocation => Hold?.ListActions?.Contains(ItemAction.UpdateLocation) ?? false;

		///<summary>Indicates if the user can activate this hold. This will never be true if the suspend option is true.</summary>
		public bool CanUserActivateHold => Hold?.ListActions?.Contains(ItemAction.Activate) ?? false;

		///<summary>Indicates if the user can change the date a hold will be automatically activated.</summary>
		public bool CanUserChangeSuspendDate => (Hold?.ListActions?.Contains(ItemAction.UpdateSuspend) ?? false) && Hold.SuspendEndDate.HasValue;

		///<summary>A list of all possible libraries a user can switch to.</summary>
		public List<Library> ListPossibleLibraries {
			get {
				return EnumUtils
					.GetAll<Library>()
					//No Courier as an option.
					.Where(x => x != Library.Unknown && x != Library.WCCLSCourier)
					.ToList();
			}
		}

		///<summary>The hold in its current state.</summary>
		[BindableProperty(nameof(CanUserCancelHold), nameof(CanUserChangePickupLocation), nameof(CanUserSuspendHold), nameof(CanUserActivateHold),
			nameof(CanUserChangeSuspendDate))]
		public Hold Hold {
			get { return GetBindableProperty(() => Hold); }
			set { SetBindableProperty(() => Hold, value); }
		}

		///<summary>The currently selected library.</summary>
		public Library LibraryCur {
			get { return GetBindableProperty(() => LibraryCur); }
			set { SetBindableProperty(() => LibraryCur, value); }
		}

		///<summary>The date that the item will be automatically renewed.</summary>
		public DateTime SuspendDate {
			get { return GetBindableProperty(() => SuspendDate); }
			set { SetBindableProperty(() => SuspendDate, value); }
		}

		///<summary>Indicates if this popup is currently making a web call.</summary>
		public bool IsLoading {
			get { return GetBindableProperty(() => IsLoading); }
			set { SetBindableProperty(() => IsLoading, value); }
		}

		public HoldOptionsPopupPageVM(INavigationService navService, IPageDialogService pageDialogSerivce, IEventAggregator eventAggregator,
			IClock clock, IWcclsApiService wcclsApi)
			: base(navService)
		{
			_pageDialogService = pageDialogSerivce;
			_eventAggregator = eventAggregator;
			_clock = clock;
			_wcclsApi = wcclsApi;
		}

		public override Task InitializeVMAsync(INavigationParameters parameters) {
			if(parameters == null || !parameters.ContainsKey(HOLD_KEY) || !parameters.ContainsKey(USER_KEY)) {
				return Task.CompletedTask;
			}
			_user = parameters.GetValue<User>(USER_KEY);
			Hold = parameters.GetValue<Hold>(HOLD_KEY);
			LibraryCur = Hold.PickupLocation;
			SuspendDate = Hold.SuspendEndDate ?? DateTime.MinValue;
			return Task.CompletedTask;
		}

		public override async Task NavigationTo(INavigationParameters parameters) {
			if(parameters == null || !parameters.ContainsKey(PauseHoldPopupPageVM.DATE_KEY)) {
				return;
			}
			await SuspendHold(parameters.GetValue<DateTime>(PauseHoldPopupPageVM.DATE_KEY), "Failed to pause hold. ");
		}

		///<summary>Handles when the user wants to change the library that an item is being picked up.</summary>
		public IAsyncCommand LibraryChangedCommand => GetCommandAsync(() => LibraryChangedCommand, true, async () => {
			//It's the same as the original.
			if(LibraryCur == Hold.PickupLocation) {
				return;
			}
			if(!await _pageDialogService.DisplayAlertAsync("Confirm", $"Are you sure you want to change the pickup " +
				$"location to {LibraryItemParser.GetLibraryName(LibraryCur)}?", "Yes", "No"))
			{
				LibraryCur = Hold.PickupLocation;
				return;
			}
			IsLoading = true;
			//Change the location.
			string error = await _wcclsApi.UpdateHoldPickupLocation(_user, new List<Hold> { Hold }, LibraryCur);
			IsLoading = false;
			if(!string.IsNullOrWhiteSpace(error)) {
				await _pageDialogService.DisplayAlertAsync("Error", "Failed to update hold's location. " + error, "Ok");
				LibraryCur = Hold.PickupLocation;
				return;
			}
			//Success.
			await RefreshAndClose();
		});

		///<summary>Handles when the user wants to suspend/pause their hold.</summary>
		public IAsyncCommand SuspendHoldCommand => GetCommandAsync(() => SuspendHoldCommand, true, async () => {
			await _navigationService.NavigateAsync(nameof(PauseHoldPopupPage));
			//The callback when we have returned from that popup will handle running the api call once we know the date.
		});

		///<summary>Handles when the user wants to reactivate this suspended hold.</summary>
		public IAsyncCommand ActivateHoldCommand => GetCommandAsync(() => ActivateHoldCommand, true, async () => {
			if(!CanUserActivateHold) {
				return;
			}
			if(!await _pageDialogService.DisplayAlertAsync("Confirm", $"Are you sure you want to activate this hold?", "Yes", "No")) {
				return;
			}
			IsLoading = true;
			string error = await _wcclsApi.ActivateHolds(_user, new List<Hold> { Hold });
			IsLoading = false;
			if(!string.IsNullOrWhiteSpace(error)) {
				await _pageDialogService.DisplayAlertAsync("Error", "Failed to activate this hold. " + error, "Ok");
				return;
			}
			await RefreshAndClose();
		});

		///<summary>Handles when the user wants to cancel their hold.</summary>
		public IAsyncCommand CancelHoldCommand => GetCommandAsync(() => CancelHoldCommand, true, async () => {
			if(!await _pageDialogService.DisplayAlertAsync("Confirm", $"Are you sure you want to cancel this hold?", "Yes", "No")) {
				return;
			}
			IsLoading = true;
			string error = await _wcclsApi.CancelHolds(_user, new List<Hold> { Hold });
			IsLoading = false;
			if(!string.IsNullOrWhiteSpace(error)) {
				await _pageDialogService.DisplayAlertAsync("Error", "Failed to cancel this hold. "+error, "Ok");
				return;
			}
			await RefreshAndClose();
		});

		///<summary>Occurs when the user changed the suspend date of their hold.</summary>
		public IAsyncCommand SuspendDateChangedCommand => GetCommandAsync(() => SuspendDateChangedCommand, true, async () => {
			if(!Hold.SuspendEndDate.HasValue || SuspendDate.Date == Hold.SuspendEndDate.Value.Date) {
				return;
			}
			if(SuspendDate.Date <= _clock.Today) {
				await _pageDialogService.DisplayAlertAsync("Invalid", "Please select a date that is after today.", "Ok");
				SuspendDate = Hold.SuspendEndDate.Value;
				return;
			}
			await SuspendHold(SuspendDate, "Failed to update the activation date for this hold. ");
		});

		public IAsyncCommand CloseCommand => GetCommandAsync(() => CloseCommand, true, async () => {
			await _navigationService.ClearPopupStackAsync();
		});

		///<summary>Tries to suspend the hold or update the hold to the given date. Will close the dialog if successful.</summary>
		private async Task SuspendHold(DateTime suspendDate, string errorMessage) {
			IsLoading = true;
			string error = await _wcclsApi.SuspendHolds(_user, new List<Hold> { Hold }, suspendDate);
			IsLoading = false;
			if(!string.IsNullOrWhiteSpace(error)) {
				await _pageDialogService.DisplayAlertAsync("Error", errorMessage+error, "Ok");
				return;
			}
			await RefreshAndClose();
		}

		///<summary>Forces the holds to be refreshed and closes the given popup.</summary>
		private async Task RefreshAndClose() {
			//Force a refresh.
			_eventAggregator.GetEvent<RefreshHoldsEvent>().Publish();
			await _navigationService.ClearPopupStackAsync();
		}

	}
}
