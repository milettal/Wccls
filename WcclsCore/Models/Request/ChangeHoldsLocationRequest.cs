using System.Collections.Generic;
using WcclsCore.Models;

namespace Core.Wccls.Models.Request {
	public class ChangeHoldsLocationRequest {

		public List<string> ListHoldIds { get; set; }

		public Library NewLocation { get; set; }
	}
}
