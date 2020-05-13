using System;
using System.Collections.Generic;

namespace WcclsCore.Models.Result {

	public class HoldsResult {

		///<summary>The number of holds that are currently paused/suspended.</summary>
		public int PausedHolds { get; set; }
		///<summary>The number of currently active holds.</summary>
		public int ActiveHolds { get; set; }
		///<summary>The total number of holds that are ready for pickup.</summary>
		public int ReadyForPickup { get; set; }
		///<summary>A list of items that are currently on hold.</summary>
		public List<Hold> ListHolds { get; set; }
		
	}

}
