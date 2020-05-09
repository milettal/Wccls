using Xamarin.Forms;
using WcclsApp.Views;

namespace WcclsApp {
	public partial class App : Application {

		public App() {
			InitializeComponent();
			MainPage = new MainPage();
		}

		protected override void OnStart() {
		}

		protected override void OnSleep() {
		}

		protected override void OnResume() {
		}
	}
}
