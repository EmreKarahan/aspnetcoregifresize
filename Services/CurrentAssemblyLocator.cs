using Microsoft.AspNetCore.SignalR.Hubs;
using System.Reflection;
using System.Collections.Generic;

namespace GifResize.Services {
	public class CurrentAssemblyLocator : IAssemblyLocator {
		public IList<Assembly> GetAssemblies() {
			return new[] { Assembly.GetEntryAssembly() };
		}
	}
}