using System;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Plugin.Popups;
using Prism.Unity;
using WcclsMobile.Pages;
using WcclsMobile.Services;
using Xamarin.Forms;

namespace WcclsMobile {
	public partial class App : PrismApplication {

		public App() : base(null) { }

		public App(IPlatformInitializer platformInitializer) : base(platformInitializer) {

		}

		protected override async void OnInitialized() {
			InitializeComponent();
			await NavigationService.NavigateAsync($"{nameof(MainTabbedPage)}?{KnownNavigationParameters.CreateTab}={nameof(NavigationPage)}|{nameof(SearchPage)}" +
				$"&{KnownNavigationParameters.CreateTab}={nameof(NavigationPage)}|{nameof(CheckedOutPage)}"+
				$"&{KnownNavigationParameters.CreateTab}={nameof(NavigationPage)}|{nameof(HoldsPage)}" +
				$"&{KnownNavigationParameters.CreateTab}={nameof(NavigationPage)}|{nameof(FinesPage)}" +
				$"&{KnownNavigationParameters.CreateTab}={nameof(NavigationPage)}|{nameof(MenuPage)}");
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry) {
			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<MainTabbedPage, MainTabbedPageVM>();
			containerRegistry.RegisterForNavigation<SearchPage, SearchPageVM>();
			containerRegistry.RegisterForNavigation<CheckedOutPage, CheckedOutPageVM>();
			containerRegistry.RegisterForNavigation<FinesPage, FinesPageVM>();
			containerRegistry.RegisterForNavigation<HoldsPage, HoldsPageVM>();
			containerRegistry.RegisterForNavigation<MenuPage, MenuPageVM>();
			containerRegistry.RegisterForNavigation<AccountsPage, AccountsPageVM>();
			containerRegistry.RegisterForNavigation<AddUser, AddUserVM>();
			containerRegistry.RegisterSingleton<IUserAuthenticationService, UserAuthenticationService>();
			containerRegistry.RegisterSingleton<IWcclsApiService, WcclsApiService>();
			containerRegistry.RegisterPopupNavigationService();
		}
	}
}