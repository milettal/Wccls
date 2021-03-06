﻿using System;
using System.Collections.Generic;

namespace WcclsCore.Models {

	public class CheckedOutItem {

		///<summary>A unique ID for this checked out item.</summary>
		public string Id { get; set; }
		///<summary>A list of actions that can be taken on this item.</summary>
		public List<ItemAction> ListActions { get; set; }
		///<summary>The status of this checked out item.</summary>
		public CheckoutStatus Status { get; set; }
		///<summary>The number of times this item has been renewed.</summary>
		public int TimesRenewed { get; set; }
		///<summary>The amount of fines this item has at this point.</summary>
		public double Fines { get; set; }
		///<summary>The due date of this item.</summary>
		public DateTime DueDate { get; set; }
		///<summary>The call number for this specific checked out item. Differs from the library item's call number.</summary>
		public string CallNumber { get; set; }
		///<summary>The item that is checked out.</summary>
		public LibraryItem LibraryItem { get; set; }
	}

	public enum CheckoutStatus {
		Unknown,
		Out,
		Coming_due,
		Overdue,
	}
}
