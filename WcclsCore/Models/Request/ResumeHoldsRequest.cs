using System;
using System.Collections.Generic;

namespace Core.Wccls.Models.Request {
	public class ResumeHoldsRequest {

		///<summary>A list of the hold ids to resume.</summary>
		public List<string> ListHoldIds { get; set; }
	}
}
