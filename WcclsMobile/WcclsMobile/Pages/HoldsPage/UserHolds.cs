using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using Prism.Services;
using WcclsCore.Models;
using WcclsCore.Models.Result;
using WcclsMobile.Models;

namespace WcclsMobile.Pages {
	public class UserHolds : ViewModelBase {

		///<summary>The user for this hold.</summary>
		public User User {
			get { return GetBindableProperty(() => User); }
			set { SetBindableProperty(() => User, value); }
		}

		///<summary>The holds for this user.</summary>
		public HoldsResult Holds {
			get { return GetBindableProperty(() => Holds); }
			set { SetBindableProperty(() => Holds, value); }
		}

		public UserHolds(User user, HoldsResult result, INavigationService navService) : base(navService) {
			User = user;
			Holds = result;
		}

		public IAsyncCommand<LibraryItem> ViewDescriptionCommand => GetCommandAsync(() => ViewDescriptionCommand, true, async (item) => {
			if(item == null) {
				return;
			}
			await _navigationService.NavigateAsync(nameof(SummaryPopupPage), (SummaryPopupPageVM.ITEM_KEY, item));
		});

		public IAsyncCommand<Hold> HoldOptionsCommand => GetCommandAsync(() => HoldOptionsCommand, true, async (hold) => {
			if(hold?.ListActions == null || hold.ListActions.Count == 0) {
				return;
			}
			await _navigationService.NavigateAsync(nameof(HoldOptionsPopupPage), (HoldOptionsPopupPageVM.HOLD_KEY, hold));
		});
	}
}
