using System;
using System.Threading.Tasks;
using Core.Xamarin.MVVM;
using Prism.Navigation;

namespace WcclsMobile.Pages {

	public class FinesDetailsPageVM : ViewModelBase {

		public const string USER_FINES_KEY = "USER_FINES_KEY";

		///<summary>The fines for this user.</summary>
		public UserFines UserFines {
			get { return GetBindableProperty(() => UserFines); }
			set { SetBindableProperty(() => UserFines, value); }
		}

		public FinesDetailsPageVM(INavigationService navigationService) : base(navigationService) {

		}

		public override Task InitializeVMAsync(INavigationParameters parameters) {
			if(parameters==null || !parameters.ContainsKey(USER_FINES_KEY)) {
				throw new ApplicationException($"Invalid inputs to {nameof(FinesDetailsPageVM)}");
			}
			UserFines = parameters.GetValue<UserFines>(USER_FINES_KEY);
			return Task.CompletedTask;
		}

	}
}
