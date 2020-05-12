using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Xamarin.MVVM;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using WcclsCore.Models;
using WcclsMobile.Events;

namespace WcclsMobile.Pages {

	public class HoldOptionsPopupPageVM : ViewModelBase {

		public const string HOLD_KEY = "HOLD_KEY";

		private IPageDialogService _pageDialogService { get; }

		private IEventAggregator _eventAggregator { get; }

		///<summary>Indicates if the user can cancel this hold.</summary>
		public bool CanUserCancelHold => Hold?.ListActions?.Contains(ItemAction.Cancel) ?? false;

		///<summary>Indicates if the user can suspend/pause this hold.</summary>
		public bool CanUserSuspendHold => Hold?.ListActions?.Contains(ItemAction.Suspend) ?? false;

		///<summary>Indicates if the user can change the pickup location for this hold.</summary>
		public bool CanUserChangePickupLocation => Hold?.ListActions?.Contains(ItemAction.UpdateLocation) ?? false;

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
		[BindableProperty(nameof(CanUserCancelHold), nameof(CanUserChangePickupLocation), nameof(CanUserSuspendHold))]
		public Hold Hold {
			get { return GetBindableProperty(() => Hold); }
			set { SetBindableProperty(() => Hold, value); }
		}

		///<summary>The currently selected library.</summary>
		public Library LibraryCur {
			get { return GetBindableProperty(() => LibraryCur); }
			set { SetBindableProperty(() => LibraryCur, value); }
		}

		public HoldOptionsPopupPageVM(INavigationService navService, IPageDialogService pageDialogSerivce, IEventAggregator eventAggregator) : base(navService) {
			_pageDialogService = pageDialogSerivce;
			_eventAggregator = eventAggregator;
		}

		public override Task InitializeVMAsync(INavigationParameters parameters) {
			if(parameters == null || !parameters.ContainsKey(HOLD_KEY)) {
				return Task.CompletedTask;
			}
			Hold = parameters.GetValue<Hold>(HOLD_KEY);
			LibraryCur = Hold.PickupLocation;
			return Task.CompletedTask;
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
			//Change the location.

			await RefreshAndClose();
		});

		///<summary>Handles when the user wants to suspend/pause their hold.</summary>
		public IAsyncCommand SuspendHoldCommand => GetCommandAsync(() => SuspendHoldCommand, true, async () => {
			if(!await _pageDialogService.DisplayAlertAsync("Confirm", $"Are you sure you want to pause this hold?", "Yes", "No")) {
				return;
			}


			await RefreshAndClose();
		});

		///<summary>Handles when the user wants to cancel their hold.</summary>
		public IAsyncCommand CancelHoldCommand => GetCommandAsync(() => CancelHoldCommand, true, async () => {
			if(!await _pageDialogService.DisplayAlertAsync("Confirm", $"Are you sure you want to cancel this hold?", "Yes", "No")) {
				return;
			}


			await RefreshAndClose();
		});

		public IAsyncCommand CloseCommand => GetCommandAsync(() => CloseCommand, true, async () => {
			await _navigationService.ClearPopupStackAsync();
		});

		///<summary>Forces the holds to be refreshed and closes the given popup.</summary>
		private async Task RefreshAndClose() {
			//Force a refresh.
			_eventAggregator.GetEvent<RefreshHoldsEvent>().Publish();
			await _navigationService.ClearPopupStackAsync();
		}

	}
}
