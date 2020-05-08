using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace WcclsCore.Models {
	///<summary>A class that represents the statistics on a users borrowing metrics.</summary>
	public class Borrowing {

		///<summary>The number of items on hold that are ready to pick up now.</summary>
		[JsonProperty("ready")]
		public int? HoldsReady { get; set; }
		///<summary>The number of items on hold that are currently in transit.</summary>
		[JsonProperty("in_transit")]
		public int? HoldsInTransit { get; set; }
		///<summary>The total number of items on hold.</summary>
		[JsonProperty("total_holds")]
		public int? TotalHolds { get; set; }
		///<summary>The total number of items checked out.</summary>
		[JsonProperty("checked_out")]
		public int? CheckedOut { get; set; }
		///<summary>The number of items that are coming due soon.</summary>
		[JsonProperty("coming_due")]
		public int? ComingDue { get; set; }
		///<summary>The number of items that are currently overdue.</summary>
		[JsonProperty("overdue")]
		public int? Overdue { get; set; }
		///<summary>Indicates if the recently returned feature is enabled.</summary>
		[JsonProperty("enable_recently_returned")]
		public bool? RecentlyReturnedEnabled { get; set; }
		///<summary>Indicates if the recently returned feature is enabled.</summary>
		[JsonProperty("recently_returned_last_date_count")]
		public int? RecentlyReturnedLastDateCount { get; set; }
		///<summary>Indicates if the recently returned feature is enabled.</summary>
		[JsonProperty("recently_returned_total_count")]
		public int? RecentlyReturnedTotalCount { get; set; }
		///<summary>The next time something is due.</summary>
		[JsonProperty("next_due")]
		public string NextDueDate { get; set; }
		///<summary>The total number of fines an account has.</summary>
		public double Fines { get; set; }
	}
}
