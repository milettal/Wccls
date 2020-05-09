using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Prism;
using Prism.Ioc;
using Rg.Plugins.Popup;

namespace WcclsMobile.Droid {
	[Activity(Label = "WCCLS", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
        protected override void OnCreate(Bundle bundle) {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            Popup.Init(this, bundle);
            CachedImageRenderer.Init(null);
            LoadApplication(new App(new AndroidInitializer()));
        }

		public override void OnBackPressed() {
            Popup.SendBackPressed(base.OnBackPressed);
		}
	}

    public class AndroidInitializer : IPlatformInitializer {
        public void RegisterTypes(IContainerRegistry containerRegistry) {
            // Register any platform specific implementations
        }
    }
}

