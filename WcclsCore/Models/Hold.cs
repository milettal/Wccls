using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WcclsCore.Models {

	///<summary>Represents a single on hold item.</summary>
	public class Hold {

		///<summary>The unique identifier for this hold.</summary>
		public string HoldId { get; set; }
		///<summary>A list of actions that can be taken with this item.</summary>
		public List<ItemAction> ListActions { get; set; }
		///<summary>The position in line this person is for this item.</summary>
		public int HoldPosition { get; set; }
		///<summary>The number of copies that are currently available, aka sitting on the shelf.</summary>
		public int AvailableCopies { get; set; }
		///<summary>The total number of copies for this item.</summary>
		public int TotalCopies { get; set; }
		///<summary>The status of this hold.</summary>
		public string Status { get; set; }
		///<summary>The date the hold expires.</summary>
		public DateTime ExpiryDate { get; set; }
		///<summary>The library that will house the item to be picked up.</summary>
		public Library PickupLocation { get; set; }
		///<summary>The item that is being held.</summary>
		public LibraryItem Item { get; set; }
	}

	public enum ItemAction {
		Unknown,
		Cancel,
		Suspend,
		UpdateLocation,
		Renew,
	}

}
