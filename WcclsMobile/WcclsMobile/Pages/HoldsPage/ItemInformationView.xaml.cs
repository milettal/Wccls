using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace WcclsMobile.Pages {
	public partial class ItemInformationView : ContentView {

		public static readonly BindableProperty DescriptionSelectedCommandProperty = BindableProperty.Create(nameof(DescriptionSelectedCommand), typeof(ICommand),
			typeof(ItemInformationView), null, BindingMode.OneWay);
		public static readonly BindableProperty ShowDescriptionProperty = BindableProperty.Create(nameof(ShowDescription), typeof(bool),
			typeof(ItemInformationView), true, BindingMode.OneWay);
		public static readonly BindableProperty CallNumberProperty = BindableProperty.Create(nameof(CallNumber), typeof(string),
			typeof(ItemInformationView), "", BindingMode.OneWay);
		public static readonly BindableProperty ShowCallNumberProperty = BindableProperty.Create(nameof(ShowCallNumber), typeof(bool),
			typeof(ItemInformationView), true, BindingMode.OneWay);

		///<summary>The command that occurs when the description is selected.</summary>
		public ICommand DescriptionSelectedCommand {
			get { return (ICommand)GetValue(DescriptionSelectedCommandProperty); }
			set { SetValue(DescriptionSelectedCommandProperty, value); }
		}

		///<summary>Indicates if the description button should be visible.</summary>
		public bool ShowDescription {
			get { return (bool)GetValue(ShowDescriptionProperty); }
			set { SetValue(ShowDescriptionProperty, value); }
		}

		///<summary>Indicates if the call number should be visible.</summary>
		public bool ShowCallNumber {
			get { return (bool)GetValue(ShowCallNumberProperty); }
			set { SetValue(ShowCallNumberProperty, value); }
		}

		///<summary>The call number of this item.</summary>
		public string CallNumber {
			get { return (string)GetValue(CallNumberProperty); }
			set { SetValue(CallNumberProperty, value); }
		}

		public ItemInformationView() {
			InitializeComponent();
		}
	}
}
