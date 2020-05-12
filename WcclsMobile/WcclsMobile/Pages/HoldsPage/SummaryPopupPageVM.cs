using System.Threading.Tasks;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using WcclsCore.Models;

namespace WcclsMobile.Pages {
	public class SummaryPopupPageVM : ViewModelBase {

		public const string ITEM_KEY = "ITEM_KEY";

		public LibraryItem Item {
			get { return GetBindableProperty(() => Item); }
			set { SetBindableProperty(() => Item, value); }
		}

		public SummaryPopupPageVM(INavigationService nav) : base(nav) {

		}

		public override Task InitializeVMAsync(INavigationParameters parameters) {
			if(parameters == null || !parameters.ContainsKey(ITEM_KEY)) {
				return Task.CompletedTask;
			}
			Item = parameters.GetValue<LibraryItem>(ITEM_KEY);
			return Task.CompletedTask;
		}

		public IAsyncCommand CloseCommand => GetCommandAsync(() => CloseCommand, true, async () => {
			await _navigationService.ClearPopupStackAsync();
		});
	}
}
