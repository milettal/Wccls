
using System;
using Core.Xamarin.MVVM;

namespace WcclsMobile.Pages {

	public class CheckedOutSummary : BindableObjectBase {

		///<summary>The total number of items for this user or group of users.</summary>
		public int TotalItems {
			get { return GetBindableProperty(() => TotalItems); }
			set { SetBindableProperty(() => TotalItems, value); }
		}

		///<summary>The total number of overdue items for this user or group of users.</summary>
		public int OverdueItems {
			get { return GetBindableProperty(() => OverdueItems); }
			set { SetBindableProperty(() => OverdueItems, value); }
		}

		///<summary>The total number of items that are coming due soon for this user or group of users.</summary>
		public int DueSoonItems {
			get { return GetBindableProperty(() => DueSoonItems); }
			set { SetBindableProperty(() => DueSoonItems, value); }
		}

		///<summary>The total number of items that are out, but not coming due soon or overdue.</summary>
		public int OutItems {
			get { return GetBindableProperty(() => OutItems); }
			set { SetBindableProperty(() => OutItems, value); }
		}

		///<summary>The time the next item is due.</summary>
		public DateTime? NextDue {
			get { return GetBindableProperty(() => NextDue); }
			set { SetBindableProperty(() => NextDue, value); }
		}

	}

}