using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Core.Dispatching
{
	/// <summary>
	/// A provider that can supply a <see cref="IDispatcher"/> implementation for the current UI thread.
	/// </summary>
	public interface IDispatcherProvider
	{
		/// <summary>
		/// Gets a <see cref="IDispatcher"/> implementation for the current UI thread.
		/// </summary>
		/// <returns>Instance of a <see cref="IDispatcher"/> implementation.</returns>
		IDispatcher? GetForCurrentThread();
	}
}
