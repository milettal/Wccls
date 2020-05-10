using Core.Xamarin.MVVM;
using WcclsCore.Models.Result;
using WcclsMobile.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WcclsMobile.Pages {

	public class UserFines : BindableObjectBase {

		public User User {
			get { return GetBindableProperty(() => User); }
			set { SetBindableProperty(() => User, value); }
		}

		public FinesResult Fines {
			get { return GetBindableProperty(() => Fines); }
			set { SetBindableProperty(() => Fines, value); }
		}

		public string PaymentUrl { get; set; }


		///<summary>Command for making a payment.</summary>
		public IAsyncCommand MakeAPaymentCommand => GetCommandAsync(() => MakeAPaymentCommand, true, async () => {
			if(string.IsNullOrWhiteSpace(PaymentUrl)) {
				return;
			}
			await Browser.OpenAsync(PaymentUrl, new BrowserLaunchOptions {
				LaunchMode = BrowserLaunchMode.SystemPreferred,
				TitleMode = BrowserTitleMode.Show,
				PreferredControlColor = Color.Red,
			});
		});

	}

}
