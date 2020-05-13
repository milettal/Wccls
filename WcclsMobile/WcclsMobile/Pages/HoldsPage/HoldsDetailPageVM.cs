using System.Threading.Tasks;
using Core.Xamarin.MVVM;
using Prism.Navigation;

namespace WcclsMobile.Pages {
	public class HoldsDetailPageVM : ViewModelBase {

		public const string USER_HOLDS_KEY = "USER_HOLDS_KEY";

		public UserHolds UserHolds {
			get { return GetBindableProperty(() => UserHolds); }
			set { SetBindableProperty(() => UserHolds, value); }
		}

		public HoldsDetailPageVM(INavigationService navService) : base(navService) {

		}

		public override Task InitializeVMAsync(INavigationParameters parameters) {
			if(parameters == null || !parameters.ContainsKey(USER_HOLDS_KEY)) {
				return Task.CompletedTask;
			}
			UserHolds = parameters.GetValue<UserHolds>(USER_HOLDS_KEY);
			return Task.CompletedTask;
		}
	}
}
