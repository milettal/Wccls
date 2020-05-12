using System;
using System.Threading.Tasks;
using Core.Wccls.Models.Result;
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
	}
}
