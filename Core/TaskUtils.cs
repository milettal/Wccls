using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core {
	public static class TaskUtils {

		///<summary>Returns a task that completes when all the passed in tasks complete.</summary>
		public static Task WhenAll(List<Func<Task>> listTasks) {
			return Task.WhenAll(listTasks.Select(x => x()));
		}
	}
}
