using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Wccls.Models.Result;
using WcclsCore.Models;
using WcclsCore.Models.Result;
using WcclsMobile.Models;

namespace WcclsMobile.Services {
	///<summary>This interface defines all web methods available for the WCCLS backend.</summary>
	public interface IWcclsApiService {

		///<summary>Returns the url for making payments. At least one other login request/ web call must be made before calling this.</summary>
		string GetPaymentUrl();

		///<summary>Attempts to log the user in with the given username and password. If there is an error,
		///the error string will be populated. Otherwise, the result will be returned.</summary>
		Task<(string error, LoginResult result)> Login(string username, string password);

		///<summary>Returns either an error or the Fines for this user.</summary>
		Task<(string error, FinesResult result)> Fines(User user);

		///<summary>Returns either an error or all the holds for this user.</summary>
		Task<(string error, HoldsResult result)> Holds(User user);

		///<summary>Returns an error or blank if successful. Cancels the given holds.</summary>
		Task<string> CancelHolds(User user, List<Hold> listHolds);

		///<summary>Suspends OR changed the date for the given list of holds to the given date. Returns an error if something went wrong.</summary>
		Task<string> SuspendHolds(User user, List<Hold> listHolds, DateTime timeSuspend);

		///<summary>Reactivates the given holds. Returns an error if something went wrong.</summary>
		Task<string> ActivateHolds(User user, List<Hold> listHolds);

		///<summary>Updates the given holds pickup location to the passed in location. Returns an error if something went wrong.</summary>
		Task<string> UpdateHoldPickupLocation(User user, List<Hold> listHolds, Library newPickupLocation);
	}
}
