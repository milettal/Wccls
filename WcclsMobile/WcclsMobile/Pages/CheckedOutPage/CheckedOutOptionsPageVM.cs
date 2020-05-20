using System;
using System.Threading.Tasks;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using WcclsCore.Models;

namespace WcclsMobile.Pages {

	public class CheckedOutOptionsPageVM : ViewModelBase {

		public const string CHECKEDOUT_KEY = "CHECKEDOUT_KEY";

		///<summary>The checked out item in question.</summary>
		[BindableProperty(nameof(CanRenewItem),nameof(CanCheckInItem))]
		public CheckedOutItem CheckedOutItem {
			get { return GetBindableProperty(() => CheckedOutItem); }
			set { SetBindableProperty(() => CheckedOutItem, value); }
		}

		///<summary>Indicates if this item can be renewed.</summary>
		public bool CanRenewItem => CheckedOutItem?.ListActions?.Contains(ItemAction.Renew) ?? false;

		///<summary>Indicates if this item can be checked in. Only electronic items.</summary>
		public bool CanCheckInItem => CheckedOutItem?.ListActions?.Contains(ItemAction.CheckIn) ?? false;

		///<summary>Indicates that we are currently trying to checkin/renew an item.</summary>
		public bool IsLoading {
			get { return GetBindableProperty(() => IsLoading, false); }
			set { SetBindableProperty(() => IsLoading, value); }
		}

		public CheckedOutOptionsPageVM(INavigationService navService) : base(navService) {

		}

		public override Task InitializeVMAsync(INavigationParameters parameters) {
			if(parameters == null || !parameters.ContainsKey(CHECKEDOUT_KEY)) {
				return Task.CompletedTask;
			}
			CheckedOutItem = parameters.GetValue<CheckedOutItem>(CHECKEDOUT_KEY);
			return Task.CompletedTask;
		}

		public IAsyncCommand RenewCommand => GetCommandAsync(() => RenewCommand, true, async () => {
			if(!CanRenewItem) {
				return;
			}
			IsLoading = true;
			await Task.Delay(2000);
			IsLoading = false;
		});

		public IAsyncCommand CheckInCommand => GetCommandAsync(() => CheckInCommand, true, async () => {
			if(!CanCheckInItem) {
				return;
			}
			IsLoading = true;
			await Task.Delay(2000);
			IsLoading = false;

		});

		public IAsyncCommand CloseCommand => GetCommandAsync(() => CloseCommand, true, async () => {
			await _navigationService.ClearPopupStackAsync();
		});

	}

}
