using System;
using System.Collections.Generic;

namespace Core.Wccls.Models.Request {

	public class PauseHoldsRequest {

		///<summary>All the holds to suspend.</summary>
		public List<string> ListHoldIds { get; set; }

		///<summary>The date that these holds will resume.</summary>
		public DateTime EndDate { get; set; }

		///<summary>Indicates if the current hold is active.</summary>
		public bool IsCurrentlyActive { get; set; }

	}
}
