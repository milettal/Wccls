using System;
using System.Collections.Generic;

namespace WcclsCore.Models.Result {

	public class CheckedOutResult {

		///<summary>The total number of items checked out.</summary>
		public int TotalCheckedOut { get; set; }

		///<summary>The date of the next due item.</summary>
		public DateTime? NextDueDate { get; set; }

		///<summary>A list of all checked out items.</summary>
		public List<CheckedOutItem> ListCheckedOutItems { get; set; }

	}

}
