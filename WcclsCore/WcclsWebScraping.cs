using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WcclsCore.Models;
using WcclsCore.Models.Result;

namespace WcclsCore {

	public class WcclsWebScraping {

		///<summary>The URL for the login method for WCCLS.</summary>
		private const string LOGIN_URL = "https://wccls.bibliocommons.com/user/login?destination=%2Fuser_dashboard";
		///<summary>The URL for the global activity api call. This is how we can tell if the user successfully logged in or not.</summary>
		private const string GLOBAL_ACTIVITY = "https://wccls.bibliocommons.com/user/1456585467/privacy_settings/global_activity_feed";
		///<summary>The URL for the fines portion of the borrowing object.</summary>
		private const string BORROWING_FINES_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=fines";
		///<summary>The URL for the checked out portion of the borrowing object.</summary>
		private const string BORROWING_CHECKED_OUT_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=checkedout";
		///<summary>The URL for the holds portion of the borrowing object.</summary>
		private const string BORROWING_HOLDS_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=holds";
		///<summary>The URL for the recentely returned portion of the borrowing object.</summary>
		private const string BORROWING_RECENTLY_RETURNED_URL = "https://wccls.bibliocommons.com/user_stats/borrowing?section=recently_returned";
		///<summary>The URL to retrieve detailed fines information.</summary>
		private const string FINES_URL = "https://gateway.bibliocommons.com/v2/libraries/wccls/fines";
		///<summary>The URL to get information about specific holds.</summary>
		private const string HOLDS_URL = "https://gateway.bibliocommons.com/v2/libraries/wccls/holds";
		///<summary>The URL for getting checked out items.</summary>
		private const string CHECKED_OUT_URL = "https://gateway.bibliocommons.com/v2/libraries/wccls/checkouts";
		///<summary>The key for the session id in the Set-Cookie response.</summary>
		private const string SESSION_ID_KEY = "_live_bcui_session_id";
		///<summary>The singleton instance for this HttpClient.</summary>
		private static HttpClient _client { get; }
		///<summary>The cookie container for the client.</summary>
		private static CookieContainer _cookieContainer { get; }

		static WcclsWebScraping() {
			_cookieContainer=new CookieContainer();
			_client=new HttpClient(new HttpClientHandler { CookieContainer=_cookieContainer });
		}

		///<summary>Attempts to log the user in with the given username or password. If this fails, it will return an empty string.</summary>
		///<param name="username">The username for this account.</param>
		///<param name="password">The password for this account.</param>
		///<exception cref="ArgumentException">Thrown if the username or password are blank.</exception>
		///<returns>The access token for this login.</returns>
		public async Task<string> Login(string username ,string password) {
			if(string.IsNullOrWhiteSpace(username)) {
				throw new ArgumentException("Username is required." ,nameof(username));
			}
			if(string.IsNullOrWhiteSpace(password)) {
				throw new ArgumentException("Password is required." ,nameof(password));
			}
			Dictionary<string, string> formData = new Dictionary<string, string>();
			formData["utf8"]="✓";
			formData["name"]=username;
			formData["user_pin"]=password;
			formData["local"]="false";
			HttpResponseMessage loginResponse = await _client.PostAsync(LOGIN_URL, new FormUrlEncodedContent(formData));
			if(!loginResponse.IsSuccessStatusCode) {
				return "";
			}
			HttpResponseMessage globalFeedResponse = await _client.GetAsync(GLOBAL_ACTIVITY);
			globalFeedResponse.EnsureSuccessStatusCode();
			try {
				string json = await globalFeedResponse.Content.ReadAsStringAsync();
				var globalFeed = JsonConvert.DeserializeAnonymousType(json, new {
					logged_in = false,
					success = false,
				});
				//Reporting us as not logged in.
				if(!globalFeed.logged_in||!globalFeed.success) {
					return "";
				}
			}
			catch(Exception e) {
				//TODO: log.
				//Couldn't deserialize the JSON meaning the login failed.
				return "";
			}
			return HttpUtils.GetSetCookieParams(loginResponse)
				.Where(x => x.Name==SESSION_ID_KEY)
				.Select(x => x.Value)
				.FirstOrDefault()??"";
		}

		///<summary>Returns the borrowing object for the logged in user. Will return null if any part of the object fails.</summary>
		///<returns>The borrowing object. Null if a failure occurs.</returns>
		public async Task<Borrowing> GetBorrowing() {
			Borrowing borrowingComplete = new Borrowing();
			bool anyFailed = false;
			async Task<Borrowing> GetBorrow(string url) {
				try {
					HttpResponseMessage response = await _client.GetAsync(url);
					string json = await response.Content.ReadAsStringAsync();
					Borrowing borrow = JsonConvert.DeserializeObject<Borrowing>(json);
					return borrow;
				}
				catch(Exception e) {
					anyFailed=true;
					return null;
				}
			}
			List<Func<Task>> listTasks = new List<Func<Task>>();
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_CHECKED_OUT_URL);
				borrowingComplete.CheckedOut=borrow?.CheckedOut;
				borrowingComplete.ComingDue=borrow?.ComingDue;
				borrowingComplete.Overdue=borrow?.Overdue;
				borrowingComplete.NextDueDate=borrow?.NextDueDate;
			});
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_HOLDS_URL);
				borrowingComplete.HoldsReady=borrow?.HoldsReady;
				borrowingComplete.HoldsInTransit=borrow?.HoldsInTransit;
				borrowingComplete.TotalHolds=borrow?.TotalHolds;
			});
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_RECENTLY_RETURNED_URL);
				borrowingComplete.RecentlyReturnedEnabled=borrow?.RecentlyReturnedEnabled??false;
				borrowingComplete.RecentlyReturnedLastDateCount=borrow?.RecentlyReturnedLastDateCount;
				borrowingComplete.RecentlyReturnedTotalCount=borrow?.RecentlyReturnedTotalCount;
			});
			listTasks.Add(async () => {
				Borrowing borrow = await GetBorrow(BORROWING_FINES_URL);
				borrowingComplete.Fines=borrow?.Fines??0;
			});
			await TaskUtils.WhenAll(listTasks);
			return anyFailed ? null : borrowingComplete;
		}

		///<summary>Returns the borrowing object for the logged in user. Will return null if any part of the object fails.</summary>
		///<returns>The borrowing object. Null if a failure occurs.</returns>
		public async Task<FinesResult> GetFines(string accountId) {
			_client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
			HttpResponseMessage message = await _client.GetAsync($"{FINES_URL}?accountId={accountId}&locale=en-US");
			if(!message.IsSuccessStatusCode) {
				return null;
			}
			string json = await message.Content.ReadAsStringAsync();
			try {
				var result = JsonConvert.DeserializeAnonymousType(json, new {
					entities = new {
						fines = new Dictionary<string,FineItem>(),
					},
					borrowing = new {
						fines = new {
							pagination = new {
								size = 0,
								totalElements = 0,
								totalPages = 0,
								number = 0,
								test = 3,
							},
							results = new string[0],
						},
						summaries = new {
							fines = new {
								totalAccrued = (double)0,
								totalFines = (double)0,
								totalCredits = (double)0,
							},
						},
					},
				});
				return new FinesResult {
					TotalAccrued=result?.borrowing?.summaries?.fines?.totalAccrued??0 ,
					TotalFines=result?.borrowing?.summaries?.fines?.totalFines??0 ,
					TotalCredits=result?.borrowing?.summaries?.fines?.totalCredits??0 ,
					ListFines=result?.entities?.fines?.Select(x => x.Value)?.ToList()??new List<FineItem>() ,
				};
			}
			catch(Exception) {
				return null;
			}
		}

		///<summary>Returns all hold information for the given account.</summary>
		///<param name="accountId">The account ID.</param>
		///<returns>The hold information. If null, something went wrong.</returns>
		public async Task<HoldsResult> GetHolds(string accountId) {
			HttpResponseMessage response = await _client.GetAsync($"{HOLDS_URL}?accountId={accountId}&size=25&locale=en-US");
			string json = await response.Content.ReadAsStringAsync();
			Dictionary<T ,V> CreateAnonymousDictionary<T, V>(T firstType ,V anonymousType) {
				return new Dictionary<T ,V>();
			}
			try {
				#region DeserializeObject
				var result = JsonConvert.DeserializeAnonymousType(json, new {
					entities = new {
						bibs = new Dictionary<string,Bib>(),
						holds = CreateAnonymousDictionary("",new {
							actions = new string[0],
							metadataId = "",
							holdsId = "",
							bibTitle = "",
							autoCheckout = false,
							holdsPosition = 0,
							status = "",
							materialType = "",
							pickupLocation = new {
								code = "",
								name = "",
								ips = new string[0],
							},
							expiryDate = new DateTime(),
						}),
					},
					borrowing = new {
						holds = new {
							pagination = new {
								count = 0,
								page = 0,
								limit = 0,
								pages = 0,
							}
						},
						summaries = new {
							inactiveHolds = new {
								totalOperative = 0,
							},
							holds = new {
								totalOperative = 0,
							},
						},
					},
				});
				#endregion
				List<Hold> listHolds = new List<Hold>();
				foreach(var holdData in result.entities.holds) {
					if(!result.entities.bibs.ContainsKey(holdData.Value.metadataId)) {
						continue;
					}
					Bib bib = result.entities.bibs[holdData.Value.metadataId];
					Hold hold = new Hold {
						HoldId = holdData.Value.holdsId,
						ExpiryDate = holdData.Value.expiryDate,
						HoldPosition = holdData.Value.holdsPosition,
						Status = holdData.Value.status,
						AvailableCopies = bib.availability.availableCopies,
						TotalCopies = bib.availability.totalCopies,
						ListActions = holdData.Value.actions.Select(x => {
							Enum.TryParse(x??"",true,out ItemAction result);
							return result;
						}).Where(x => x != ItemAction.Unknown).ToList(),
						PickupLocation = LibraryParser.GetLibaryByCode(holdData.Value.pickupLocation.code),
						Item = GetItemFromBib(bib),
					};
					listHolds.Add(hold);
				}
				return new HoldsResult {
					ListHolds=listHolds ,
					ActiveHolds=result.borrowing.summaries.holds.totalOperative ,
					InactiveHolds=result.borrowing.summaries.inactiveHolds.totalOperative ,
				};
			}
			catch(Exception e) {
				return null;
			}
		}

		public async Task<CheckedOutResult> GetCheckedOut(string accountId) {
			if(string.IsNullOrWhiteSpace(accountId)) {
				throw new ArgumentException(nameof(accountId));
			}
			HttpResponseMessage response = await _client.GetAsync($"{CHECKED_OUT_URL}?accountId={accountId}&size=25&locale=en-US");
			if(!response.IsSuccessStatusCode) {
				return null;
			}
			Dictionary<T ,V> CreateAnonymousDictionary<T, V>(T firstType ,V anonymousType) {
				return new Dictionary<T ,V>();
			}
			try {
				string json = await response.Content.ReadAsStringAsync();
				var result = JsonConvert.DeserializeAnonymousType(json, new {
					entities = new {
						bibs = new Dictionary<string,Bib>(),
						checkouts = CreateAnonymousDictionary("", new {
							actions = new string[0],
							checkoutId = "",
							metadataId = "",
							status = "",
							timesRenewed = 0,
							fines = (double)0,
							barcode = "",
							dueDate = "",
						}),
					},
					borrowing = new {
						checkouts = new {
							pagination = new {
								count = 0,
								page = 0,
								limit = 0,
								pages = 0,
							}
						},
						summaries = new {
							checkedout = new {
								nextDue = new {
									date = "",
								},
								total = 0,
							}
						},
					},
				});
				List<CheckedOutItem> listItems = new List<CheckedOutItem>();
				foreach(var checkout in result.entities.checkouts) {
					if(!result.entities.bibs.ContainsKey(checkout.Value.metadataId)) {
						continue;
					}
					Bib bib = result.entities.bibs[checkout.Value.metadataId];
					DateTime.TryParse(checkout.Value.dueDate, out DateTime dueDate);
					CheckedOutItem checkedOut = new CheckedOutItem {
						Fines = checkout.Value.fines,
						DueDate = dueDate,
						Id = checkout.Value.checkoutId,
						ListActions = checkout.Value.actions.Select(x => {
							Enum.TryParse(x,true,out ItemAction action);
							return action;
						}).Where(x => x != ItemAction.Unknown).ToList(),
						Status = checkout.Value.status,
						TimesRenewed = checkout.Value.timesRenewed,
						LibraryItem = GetItemFromBib(bib),
					};
					listItems.Add(checkedOut);
				}
				DateTime.TryParse(result?.borrowing?.summaries?.checkedout?.nextDue?.date??"",out DateTime nextDue);
				return new CheckedOutResult {
					NextDueDate = nextDue == DateTime.MinValue ? (DateTime?)null : nextDue,
					TotalCheckedOut = result?.borrowing?.summaries?.checkedout?.total??0,
					ListCheckedOutItems = listItems,
				};
			}
			catch(Exception e) {
				return null;
			}
		}

		///<summary>Renews a list of checkout items.</summary>
		///<param name="listCheckoutIds">The list of checked out items that should be renewed.</param>
		///<param name="accountId">The account id for this account.</param>
		///<returns>A result per checkout id.</returns>
		public async Task<RenewItemsResult> RenewItems(List<string> listCheckoutIds, string accountId) {
			if(listCheckoutIds == null || listCheckoutIds.Count == 0) {
				throw new ArgumentException(accountId);
			}
			HttpResponseMessage response = await _client.PatchAsync($"{CHECKED_OUT_URL}?locale=en-US", new StringContent(JsonConvert.SerializeObject(new {
				accountId,
				checkoutIds = listCheckoutIds,
				renew = true,
			}),Encoding.UTF8,"application/json"));
			if(!response.IsSuccessStatusCode) {
				//All failed
				return new RenewItemsResult {
					DictionaryResponses = listCheckoutIds.ToDictionary(x => x,x => new RenewResult {
						WasRenewed = false,
						ErrorMessage = $"Bad Status Code: {(int)response.StatusCode} - {response.StatusCode}",
					}),
				};
			}
			try {
				string json = await response.Content.ReadAsStringAsync();
				var result = JsonConvert.DeserializeAnonymousType(json, new {
					failures = new[] {
						new {
							errorResponseDTO = new {
								classification = "",
								message = "",
							},
							id = "",
							itemId = "",
							metadataId = "",
						}
					},
				});
				Dictionary<string,RenewResult> dictResults = new Dictionary<string, RenewResult>();
				//First add them all as successful.
				foreach(string id in listCheckoutIds) {
					dictResults[id] = new RenewResult { WasRenewed = true, ErrorMessage = "" };
				}
				if(result?.failures != null) {
					foreach(var failure in result.failures) {
						if(!dictResults.ContainsKey(failure?.id??"")) {
							continue;
						}
						RenewResult renewResult = dictResults[failure.id];
						renewResult.WasRenewed=false;
						renewResult.ErrorMessage=failure?.errorResponseDTO?.message;
					}
				}
				return new RenewItemsResult {
					DictionaryResponses = dictResults,
				};
			}
			catch(Exception e) {
				//All failed
				return new RenewItemsResult {
					DictionaryResponses=listCheckoutIds.ToDictionary(x => x ,x => new RenewResult {
						WasRenewed=false ,
						ErrorMessage=$"Bad Status Code: {(int)response.StatusCode} - {response.StatusCode}" ,
					})
				};
			}
		}

		///<summary>Returns a library item from the given bib (the JSON from the libraries web call).</summary>
		private LibraryItem GetItemFromBib(Bib bib) {
			if(bib == null) {
				return null;
			}
			return new LibraryItem {
				Id=bib.id ,
				Edition=bib.briefInfo.edition ,
				AverageRating=bib.briefInfo.rating.averageRating ,
				TotalCount=bib.briefInfo.rating.totalCount ,
				CallNumber=bib.briefInfo.callNumber ,
				ContentType=bib.briefInfo.contentType ,
				Description=bib.briefInfo.description ,
				PublicationDate=bib.briefInfo.publicationDate ,
				ListAuthors=bib.briefInfo.authors?.ToList() ,
				SourceLibrary=LibraryParser.GetLibaryByCode(bib.sourceLibId.ToString()) ,
				Format=bib.briefInfo.format ,
				Subtitle=bib.briefInfo.subtitle ,
				Title=bib.briefInfo.title ,
				ListISBNs=bib.briefInfo.isbns?.ToList() ,
				ImageResources=new ItemResources {
					SmallUrl=bib.briefInfo.jacket.small ,
					MediumUrl=bib.briefInfo.jacket.medium ,
					LargeUrl=bib.briefInfo.jacket.large ,
				}
			};
		}

		private class Bib {

			public string id { get; set; }
			public string sourceLibId { get; set; }
			public BriefInfo briefInfo { get; set; }
			public Availability availability { get; set; }

			public class Availability {

				public string bibType { get; set; }
				public string availabilityLocationType { get; set; }
				public string status { get; set; }
				public string circulationType { get; set; }
				public bool libraryUseOnly { get; set; }
				public int heldCopies { get; set; }
				public int availableCopies { get; set; }
				public int totalCopies { get; set; }
				public int onOrderCopies { get; set; }
				public int volumesCount { get; set; }
				public string localisedStatus { get; set; }
				public string eresourceDescription { get; set; }
				public string eresourceUrl { get; set; }
				public string statusType { get; set; }
				public bool singleBranch { get; set; }
			}

			public class BriefInfo {

				public string title { get; set; }
				public string subtitle { get; set; }
				public string format { get; set; }
				public string[] superFormats { get; set; }
				public string consumptionFormat { get; set; }
				public string callNumber { get; set; }
				public string description { get; set; }
				public string[] genreForm { get; set; }
				public string[] subjectHeadings { get; set; }
				public string[] authors { get; set; }
				public string publicationDate { get; set; }
				public string[] isbns { get; set; }
				public string primarylanguage { get; set; }
				public string edition { get; set; }
				public string multiscriptTitle { get; set; }
				public string multiscriptAuthor { get; set; }
				public string contentType { get; set; }
				public string[] audiences { get; set; }
				public string[] compositeSubjectHeadings { get; set; }
				public Jacket jacket { get; set; }
				public Rating rating { get; set; }

				public class Jacket {
					public string small { get; set; }
					public string medium { get; set; }
					public string large { get; set; }
				}

				public class Rating {
					public int averageRating { get; set; }
					public int totalCount { get; set; }
				}
			}

		}
	}

}
