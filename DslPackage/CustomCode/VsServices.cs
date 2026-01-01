using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;

namespace Dyvenix.GenIt.DslPackage.CustomCode
{
	public static class VsServices
	{
		private static IAsyncServiceProvider _sp;

		public static void Initialize(IAsyncServiceProvider serviceProvider)
			=> _sp = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

		public static async Task<T> GetAsync<T>(Type serviceType) where T : class
		{
			if (_sp == null) throw new InvalidOperationException("VsServices.Initialize was not called.");
			return await _sp.GetServiceAsync(serviceType) as T;
		}
	}
}

