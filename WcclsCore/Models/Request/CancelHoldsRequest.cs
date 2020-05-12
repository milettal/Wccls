using System;
using System.Collections.Generic;

namespace Core.Wccls.Models.Request {

	public class CancelHoldsRequest {

		///<summary>A list of hold ids to cancel. The holds ids and the metaids should match order.</summary>
		public List<string> ListHoldIds { get; set; }

		///<summary>A list of metadata ids to cancel. The holds ids and the metaids should match order.</summary>
		public List<string> ListMetadataIds { get; set; }
	}
}
