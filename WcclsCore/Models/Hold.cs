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
		///<summary>The status of this hold as a string. Only used if we are unable to parse the ststus</summary>
		public string StatusStr { get; set; }
		///<summary>The status of the hold.</summary>
		public HoldStatus Status { get; set; }
		///<summary>The date the hold expires.</summary>
		public DateTime? ExpiryDate { get; set; }
		///<summary>The date that the hold was placed.</summary>
		public DateTime? HoldPlacedDate { get; set; }
		///<summary>The date that the hold is automatically resumed.</summary>
		public DateTime? SuspendEndDate { get; set; }
		///<summary>The library that will house the item to be picked up.</summary>
		public Library PickupLocation { get; set; }
		///<summary>The item that is being held.</summary>
		public LibraryItem Item { get; set; }
	}

	public enum HoldStatus {
		Unknown,
		[Description("Paused")]
		Suspended,
		[Description("Not yet available")]
		Not_Yet_Available,
		[Description("In transit")]
		In_Transit,
		[Description("Ready for pickup")]
		Ready_For_Pickup,
	}

	public enum ItemAction {
		Unknown,
		///<summary>Cancel a hold on an item.</summary>
		Cancel,
		///<summary>Suspend a hold on an item.</summary>
		Suspend,
		///<summary>Update the pickup location for an item.</summary>
		UpdateLocation,
		///<summary>Renew a checked out item.</summary>
		Renew,
		///<summary>Update the date on a suspended hold.</summary>
		UpdateSuspend,
		///<summary>Activate a suspended hold.</summary>
		Activate,
		///<summary>Return an item. Has to be an electronic item.</summary>
		CheckIn,
		///<summary>Not supported right now.</summary>
		ChangeFormat,
	}

}
