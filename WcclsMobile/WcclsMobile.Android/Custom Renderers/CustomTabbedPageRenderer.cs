using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using WcclsMobile.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace WcclsMobile.Droid.CustomRenderers {
	public class CustomTabbedPageRenderer : TabbedPageRenderer {

		public CustomTabbedPageRenderer(Context context) : base(context) {

		}

		[Obsolete]
		public CustomTabbedPageRenderer() : base() { }

		protected override void SetTabIconImageSource(TabLayout.Tab tab, Drawable icon) {
			base.SetTabIconImageSource(tab, icon);
			tab.SetText("");
		}
	}
}
