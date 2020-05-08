using System;
using System.Collections.Generic;

namespace WcclsCore.Models.Result {

	public class HoldsResult {

		///<summary>The number of holds that are currently inactive.</summary>
		public int InactiveHolds { get; set; }
		///<summary>The number of currently active holds.</summary>
		public int ActiveHolds { get; set; }
		///<summary>A list of items that are currently on hold.</summary>
		public List<Hold> ListHolds { get; set; }
		
	}

}
