using System;
using System.Threading.Tasks;
using Core.DependencyInjection;
using Core.Xamarin.MVVM;
using Prism.Navigation;
using Prism.Services;

namespace WcclsMobile.Pages {
	public class PauseHoldPopupPageVM : ViewModelBase {

		public const string DATE_KEY = "DATE_KEY";

		private IClock _clock { get; }

		private IPageDialogService _pageDialog { get; }

		public DateTime SuspendDate {
			get { return GetBindableProperty(() => SuspendDate, _clock.Today); }
			set { SetBindableProperty(() => SuspendDate, value); }
		}

		public PauseHoldPopupPageVM(INavigationService navService, IClock clock, IPageDialogService pageDialog) : base(navService) {
			_clock = clock;
			_pageDialog = pageDialog;
		}

		public IAsyncCommand OkCommand => GetCommandAsync(() => OkCommand, true, async () => {
			if(SuspendDate <= _clock.Today) {
				await _pageDialog.DisplayAlertAsync("Error", "Please select a date that is after today.", "Ok");
				return;
			}
			await _navigationService.GoBackAsync((DATE_KEY, SuspendDate));
		});

		///<summary>Closes the window without returning the date.</summary>
		public IAsyncCommand CancelCommand => GetCommandAsync(() => CancelCommand, true, async () => {
			await _navigationService.GoBackAsync();
		});

	}
}
