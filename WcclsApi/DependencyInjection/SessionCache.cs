using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Net;

namespace WcclsApi.DependencyInjection {
	public class SessionCache : ISessionCache {

		private ISystemClock _clock { get; }
		///<summary>The maximum number of minutes a session is valid since it was last used.</summary>
		private const int MAX_SESSION_MINUTES = 30;

		///<summary>An in memory cache of sessions.</summary>
		private Dictionary<string,(DateTime timeLastUsed,CookieContainer container,long userId)> _dictContainers { get; } 
			= new Dictionary<string, (DateTime timeLastUsed, CookieContainer container, long userId)>();

		public SessionCache(ISystemClock clock) {
			_clock = clock;
		}

		public string AddSession(CookieContainer container, long userId) {
			if(container == null) {
				throw new ArgumentNullException(nameof(container));
			}
			if(userId <= 0) {
				throw new ArgumentNullException(userId.ToString());
			}
			lock(_dictContainers) {
				string guid = "";
				while(guid == "" || _dictContainers.ContainsKey(guid)) {
					guid = Guid.NewGuid().ToString();
				}
				_dictContainers[guid] = (_clock.UtcNow.UtcDateTime, container, userId);
				return guid;
			}
		}

		public (CookieContainer container, long userId) GetCachedSession(string guid) {
			lock(_dictContainers) {
				if(!_dictContainers.ContainsKey(guid)) {
					return (null,0);
				}
				(DateTime timeLastUsed, CookieContainer container, long userId) = _dictContainers[guid];
				//Too old.
				if(timeLastUsed.AddMinutes(MAX_SESSION_MINUTES) < _clock.UtcNow.UtcDateTime) {
					_dictContainers.Remove(guid);
					return (null,0);
				}
				//Otherwise, this session is good. Lets update the time accessed.
				_dictContainers[guid] = (_clock.UtcNow.UtcDateTime, container, userId);
				return (container, userId);
			}
		}
	}
}
