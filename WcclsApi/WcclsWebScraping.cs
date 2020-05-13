using Core;
using Core.Wccls.Models.Result;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WcclsCore.Models;
using WcclsCore.Models.Result;

namespace WcclsApi {

	public class WcclsWebScraping {

		///<summary>The URL for the login method for WCCLS.</summary>
		private const string LOGIN_URL = "https://wccls.bibliocommons.com/user/login?destination=%2Fuser_dashboard";
		///<summary>Returns the user dashboard which includes the username and unique identifier of the user.</summary>
		private const string USER_DASHBOARD = "https://wccls.bibliocommons.com/user_dashboard";
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
		///<summaryThe status that indicates a hold is ready for pickup.</summary>
		private const string READY_FOR_PICKUP_KEY = "ready for pickup";
		///<summary>The instance for this HttpClient.</summary>
		private HttpClient _client { get; }
		private ISystemClock _systemClock { get; }

		public WcclsWebScraping(HttpClient client, ISystemClock systemClock) {
			_client = client;
			_systemClock = systemClock;
		}

		///<summary>Attempts to log the user in with the given username or password. If this fails, it will return an empty string.</summary>
		///<param name="username">The username for this account.</param>
		///<param name="password">The password for this account.</param>
		///<exception cref="ArgumentException">Thrown if the username or password are blank.</exception>
		///<returns>The users id.</returns>
		public async Task<(long userId, string username)> Login(string username, string password) {
			if(string.IsNullOrWhiteSpace(username)) {
				throw new ArgumentException("Username is required.", nameof(username));
			}
			if(string.IsNullOrWhiteSpace(password)) {
				throw new ArgumentException("Password is required.", nameof(password));
			}
			Dictionary<string, string> formData = new Dictionary<string, string>();
			formData["utf8"]="✓";
			formData["name"]=username;
			formData["user_pin"]=password;
			formData["local"]="false";
			HttpResponseMessage loginResponse = await _client.PostAsync(LOGIN_URL, new FormUrlEncodedContent(formData));
			if(!loginResponse.IsSuccessStatusCode) {
				return (0, "");
			}
			HttpResponseMessage userDashboard = await _client.GetAsync(USER_DASHBOARD);
			if(!userDashboard.IsSuccessStatusCode) {
				return (0, "");
			}
			//Unfortunately this is how they transmit the needed information.
			string html = await userDashboard.Content.ReadAsStringAsync();
			Match match = Regex.Match(html, "var BC_USER_ID = ([0-9]*);");
			if(match==null) {
				return (0, "");
			}
			long.TryParse(match.Groups[1].Value, out long userId);
			if(userId==0) {
				return (0, "");
			}
			userId++;
			match = Regex.Match(html, "var BC_USERNAME = \"([A-Za-z0-9]*)\";");
			if(match == null) {
				return (userId, "");
			}
			return (userId, match.Groups[1].Value);
		}

		///<summary>Cancels the holds for the given hold ids and metadata ids.</summary>
		public async Task CancelHolds(long userId, List<string> listHoldIds, List<string> listMetadataIds) {
			HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, $"{HOLDS_URL}?locale=en-US");
			message.Content = new StringContent(JsonConvert.SerializeObject(new {
				accountId = userId,
				holdIds = listHoldIds.ToArray(),
				metadataIds = listMetadataIds.ToArray(),
			}), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _client.SendAsync(message);
			if(!response.IsSuccessStatusCode) {
				throw new ApplicationException("Error: "+await response.Content.ReadAsStringAsync());
			}
		}

		///<summary>A list of suspended holds to resume.</summary>
		public async Task<ResumeHoldsResult> ResumeHolds(long userId, List<string> listHoldIds) {
			HttpResponseMessage response = await _client.PatchAsync($"{HOLDS_URL}?locale=en-US",new StringContent(JsonConvert.SerializeObject(new {
				accountId = userId,
				holdIds = listHoldIds.ToArray(),
				suspended = new {
					status = false,
				},
			}),Encoding.UTF8,"application/json"));
			if(!response.IsSuccessStatusCode) {
				return null;
			}
			string json = await response.Content.ReadAsStringAsync();
			try {
				return new ResumeHoldsResult { Success = true };
			}
			catch(Exception e) {
				return null;
			}
		}

		public async Task ChangeHoldsLocation(long userId, List<string> listHoldIds, Library newLocation) {
			HttpResponseMessage response = await _client.PatchAsync($"{HOLDS_URL}?locale=en-US", new StringContent(JsonConvert.SerializeObject(new {
				accountId = userId,
				holdIds = listHoldIds.ToArray(),
				location = LibraryItemParser.GetCodeByLibrary(newLocation),
			}), Encoding.UTF8, "application/json"));
			if(!response.IsSuccessStatusCode) {
				throw new ApplicationException($"Error: {(int)response.StatusCode} - {response.StatusCode.ToString()}");
			}
		}

		///<summary>Pauses all the given holds.</summary>
		public async Task<PauseHoldsResult> PauseHolds(long userId, List<string> listHoldIds, DateTime endDate, bool isCurrentlyActive) {
			HttpResponseMessage response = await _client.PatchAsync($"{HOLDS_URL}?locale=en-US",new StringContent(JsonConvert.SerializeObject(new {
				accountId = userId,
				holdIds = listHoldIds.ToArray(),
				suspended = new {
					startDate = (string)null,
					endDate = endDate.ToString("yyyy-MM-dd"),
					status = true,
				},
			}),Encoding.UTF8,"application/json"));
			if(!response.IsSuccessStatusCode) {
				return null;
			}
			string json = await response.Content.ReadAsStringAsync();
			try {
				return new PauseHoldsResult { Success = true };
			}
			catch(Exception e) {
				return null;
			}
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
		public async Task<FinesResult> GetFines(long userId) {
			_client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
			HttpResponseMessage message = await _client.GetAsync($"{FINES_URL}?accountId={userId}&locale=en-US");
			if(!message.IsSuccessStatusCode) {
				return null;
			}
			string json = await message.Content.ReadAsStringAsync();
			try {
				var result = JsonConvert.DeserializeAnonymousType(json, new {
					entities = new {
						fines = new Dictionary<string, FineItem>(),
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
					TotalAccrued=result?.borrowing?.summaries?.fines?.totalAccrued??0,
					TotalFines=result?.borrowing?.summaries?.fines?.totalFines??0,
					TotalCredits=result?.borrowing?.summaries?.fines?.totalCredits??0,
					ListFines=result?.entities?.fines?.Select(x => x.Value)?.ToList()??new List<FineItem>(),
				};
			}
			catch(Exception) {
				return null;
			}
		}

		///<summary>Returns all hold information for the given account.</summary>
		///<param name="userId">The account ID.</param>
		///<returns>The hold information. If null, something went wrong.</returns>
		public async Task<HoldsResult> GetHolds(long userId) {
			HttpResponseMessage response = await _client.GetAsync($"{HOLDS_URL}?accountId={userId}&size=25&locale=en-US");
			string json = await response.Content.ReadAsStringAsync();
			Dictionary<T, V> CreateAnonymousDictionary<T, V>(T firstType, V anonymousType) {
				return new Dictionary<T, V>();
			}
			try {
				#region DeserializeObject
				var result = JsonConvert.DeserializeAnonymousType(json, new {
					entities = new {
						bibs = new Dictionary<string, Bib>(),
						holds = CreateAnonymousDictionary("", new {
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
							expiryDate = (DateTime?) new DateTime(),
							suspendEndDate = (DateTime?) new DateTime(),
							holdPlacedDate = (DateTime?) new DateTime(),
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
					Enum.TryParse(holdData.Value.status, true, out HoldStatus holdStatus);
					Hold hold = new Hold {
						HoldId = holdData.Value.holdsId,
						ExpiryDate = holdData.Value.expiryDate,
						HoldPosition = holdData.Value.holdsPosition,
						StatusStr = holdData.Value.status,
						Status = holdStatus,
						AvailableCopies = bib.availability.availableCopies ?? 0,
						TotalCopies = bib.availability.totalCopies ?? 0,
						ListActions = holdData.Value.actions.Select(x => {
							Enum.TryParse(x ?? "", true, out ItemAction ac);
							return ac;
						}).Where(x => x != ItemAction.Unknown).ToList(),
						HoldPlacedDate = holdData.Value.holdPlacedDate,
						SuspendEndDate = holdData.Value.suspendEndDate,
						PickupLocation = LibraryItemParser.GetLibaryByCode(holdData.Value.pickupLocation?.code),
						Item = GetItemFromBib(bib),
					};
					listHolds.Add(hold);
				}
				return new HoldsResult {
					ListHolds = listHolds,
					ActiveHolds = listHolds.Count(x => x.Status == HoldStatus.In_Transit || x.Status == HoldStatus.Not_Yet_Available),
					PausedHolds = listHolds.Count(x => x.Status == HoldStatus.Suspended),
					ReadyForPickup = listHolds.Count(x => x.Status == HoldStatus.Ready_For_Pickup),
				};
			}
			catch(Exception e) {
				return null;
			}
		}

		///<summary>Returns all of the items currently checked out to the user.</summary>
		public async Task<CheckedOutResult> GetCheckedOut(long userId) {
			if(userId<=0) {
				throw new ArgumentException(nameof(userId));
			}
			HttpResponseMessage response = await _client.GetAsync($"{CHECKED_OUT_URL}?accountId={userId}&size=25&locale=en-US");
			if(!response.IsSuccessStatusCode) {
				return null;
			}
			Dictionary<T, V> CreateAnonymousDictionary<T, V>(T firstType, V anonymousType) {
				return new Dictionary<T, V>();
			}
			try {
				string json = await response.Content.ReadAsStringAsync();
				var result = JsonConvert.DeserializeAnonymousType(json, new {
					entities = new {
						bibs = new Dictionary<string, Bib>(),
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
						Fines=checkout.Value.fines,
						DueDate=dueDate,
						Id=checkout.Value.checkoutId,
						ListActions=checkout.Value.actions.Select(x => {
							Enum.TryParse(x, true, out ItemAction action);
							return action;
						}).Where(x => x!=ItemAction.Unknown).ToList(),
						Status=checkout.Value.status,
						TimesRenewed=checkout.Value.timesRenewed,
						LibraryItem=GetItemFromBib(bib),
					};
					listItems.Add(checkedOut);
				}
				DateTime.TryParse(result?.borrowing?.summaries?.checkedout?.nextDue?.date??"", out DateTime nextDue);
				return new CheckedOutResult {
					NextDueDate=nextDue==DateTime.MinValue ? (DateTime?)null : nextDue,
					TotalCheckedOut=result?.borrowing?.summaries?.checkedout?.total??0,
					ListCheckedOutItems=listItems,
				};
			}
			catch(Exception e) {
				return null;
			}
		}

		///<summary>Renews a list of checkout items.</summary>
		///<param name="listCheckoutIds">The list of checked out items that should be renewed.</param>
		///<param name="userId">The account id for this account.</param>
		///<returns>A result per checkout id.</returns>
		public async Task<RenewItemsResult> RenewItems(List<string> listCheckoutIds, long userId) {
			if(listCheckoutIds==null||listCheckoutIds.Count==0) {
				throw new ArgumentException(nameof(userId));
			}
			HttpResponseMessage response = await _client.PatchAsync($"{CHECKED_OUT_URL}?locale=en-US", new StringContent(JsonConvert.SerializeObject(new {
				userId,
				checkoutIds = listCheckoutIds,
				renew = true,
			}), Encoding.UTF8, "application/json"));
			if(!response.IsSuccessStatusCode) {
				//All failed
				return new RenewItemsResult {
					DictionaryResponses=listCheckoutIds.ToDictionary(x => x, x => new RenewResult {
						WasRenewed=false,
						ErrorMessage=$"Bad Status Code: {(int)response.StatusCode} - {response.StatusCode}",
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
				Dictionary<string, RenewResult> dictResults = new Dictionary<string, RenewResult>();
				//First add them all as successful.
				foreach(string id in listCheckoutIds) {
					dictResults[id]=new RenewResult { WasRenewed=true, ErrorMessage="" };
				}
				if(result?.failures!=null) {
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
					DictionaryResponses=dictResults,
				};
			}
			catch(Exception e) {
				//All failed
				return new RenewItemsResult {
					DictionaryResponses=listCheckoutIds.ToDictionary(x => x, x => new RenewResult {
						WasRenewed=false,
						ErrorMessage=$"Bad Status Code: {(int)response.StatusCode} - {response.StatusCode}",
					})
				};
			}
		}

		///<summary>Returns a library item from the given bib (the JSON from the libraries web call).</summary>
		private LibraryItem GetItemFromBib(Bib bib) {
			if(bib == null) {
				return null;
			}
			Enum.TryParse(bib.briefInfo.format ?? "", true, out ItemFormat format);
			return new LibraryItem {
				Id = bib.id,
				Edition = bib.briefInfo.edition,
				AverageRating = bib.briefInfo.rating?.averageRating ?? 0,
				TotalCount = bib.briefInfo.rating?.totalCount ?? 0,
				CallNumber = bib.briefInfo.callNumber,
				ContentType = bib.briefInfo.contentType,
				Description = bib.briefInfo.description,
				PublicationDate = bib.briefInfo.publicationDate,
				ListAuthors = bib.briefInfo.authors?.ToList(),
				SourceLibrary = LibraryItemParser.GetLibaryByCode(bib.sourceLibId.ToString()),
				Format = format,
				Subtitle = bib.briefInfo.subtitle,
				Title = bib.briefInfo.title,
				ListISBNs = bib.briefInfo.isbns?.ToList(),
				ImageResources = new ItemResources {
					SmallUrl = bib.briefInfo.jacket?.small ?? "",
					MediumUrl = bib.briefInfo.jacket?.medium ?? "",
					LargeUrl = bib.briefInfo.jacket?.large ?? "",
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
				public bool? libraryUseOnly { get; set; }
				public int? heldCopies { get; set; }
				public int? availableCopies { get; set; }
				public int? totalCopies { get; set; }
				public int? onOrderCopies { get; set; }
				public int? volumesCount { get; set; }
				public string localisedStatus { get; set; }
				public string eresourceDescription { get; set; }
				public string eresourceUrl { get; set; }
				public string statusType { get; set; }
				public bool? singleBranch { get; set; }
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
