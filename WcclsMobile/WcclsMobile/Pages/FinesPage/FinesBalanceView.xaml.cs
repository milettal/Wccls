using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace WcclsMobile.Pages {
	public partial class FinesBalanceView : ContentView {

		public static readonly BindableProperty BalanceProperty = BindableProperty.Create(nameof(Balance), typeof(double), typeof(FinesBalanceView), (double)0,
			BindingMode.OneWay);
		public static readonly BindableProperty ShowDueProperty = BindableProperty.Create(nameof(ShowDue), typeof(bool), typeof(FinesBalanceView), false,
			BindingMode.OneWay);

		///<summary>The balance for this view.</summary>
		public double Balance {
			get { return (double)GetValue(BalanceProperty); }
			set { SetValue(BalanceProperty, value); }
		}

		///<summary>Indicates if this view should show "due" after the balance.</summary>
		public bool ShowDue {
			get { return (bool)GetValue(ShowDueProperty); }
			set { SetValue(ShowDueProperty, value); }
		}

		public FinesBalanceView() {
			InitializeComponent();
		}

	}
}
