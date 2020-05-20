using System;
using System.Linq;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using WcclsCore.Models;
using WcclsCore.Models.Result;
using WcclsMobile.Models;

namespace WcclsMobile.Pages {
	public class UserCheckOuts : ViewModelBase {

		public User User {
			get { return GetBindableProperty(() => User); }
			set { SetBindableProperty(() => User, value); }
		}

		[BindableProperty(nameof(HasAnOverdueItem))]
		public CheckedOutResult CheckedOut {
			get { return GetBindableProperty(() => CheckedOut); }
			set { SetBindableProperty(() => CheckedOut, value); }
		}

		///<summary>A summary of all the items checked out for this user.</summary>
		public CheckedOutSummary CheckedOutSummary {
			get { return GetBindableProperty(() => CheckedOutSummary); }
			set { SetBindableProperty(() => CheckedOutSummary, value); }
		}

		///<summary>Indicates if this user has at least one overdue item.</summary>
		public bool HasAnOverdueItem {
			get {
				return CheckedOut?.ListCheckedOutItems?.Any(x => x.Status == CheckoutStatus.Overdue) ?? false;
			}
		}

		public UserCheckOuts(User user, CheckedOutResult result, INavigationService navService) : base(navService) {
			User = user;
			CheckedOut = result;
			CheckedOutSummary = new CheckedOutSummary {
				NextDue = result.NextDueDate,
				TotalItems = result.TotalCheckedOut,
				OutItems = result.ListCheckedOutItems.Count(x => x.Status == CheckoutStatus.Out),
				DueSoonItems = result.ListCheckedOutItems.Count(x => x.Status == CheckoutStatus.Coming_due),
				OverdueItems = result.ListCheckedOutItems.Count(x => x.Status == CheckoutStatus.Overdue),
			};
		}

		///<summary>Command that handles when the user wants to view the options for a checked out item.</summary>
		public IAsyncCommand<CheckedOutItem> ShowCheckedOutOptionsCommand => GetCommandAsync(() => ShowCheckedOutOptionsCommand, true, async (checkedOutItem) => {
			await _navigationService.NavigateAsync(nameof(CheckedOutOptionsPage), (CheckedOutOptionsPageVM.CHECKEDOUT_KEY, checkedOutItem));
		});
	}
}
