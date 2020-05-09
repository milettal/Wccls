using Core.Xamarin.MVVM;
using Prism.Navigation;

namespace WcclsMobile.Pages {
	public class MainPageVM : ViewModelBase {
		public MainPageVM(INavigationService navigationService)
			: base(navigationService) {
			Title = "Main Page";
		}
	}
}
