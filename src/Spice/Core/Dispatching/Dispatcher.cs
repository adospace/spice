using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Core.Dispatching
{
		/// <inheritdoc/>
	public partial class Dispatcher : IDispatcher
	{
		public static IDispatcher? GetForCurrentThread() =>
			DispatcherProvider.Current.GetForCurrentThread();

		/// <inheritdoc/>
		public bool IsDispatchRequired =>
#if ANDROID || IOS
			IsDispatchRequiredImplementation();
#else
			false;
#endif

		/// <inheritdoc/>
		public bool Dispatch(Action action)
		{
			_ = action ?? throw new ArgumentNullException(nameof(action));

#if ANDROID || IOS
			return DispatchImplementation(action);
#else
			return false;
#endif

		}

		/// <inheritdoc/>
		public bool DispatchDelayed(TimeSpan delay, Action action)
		{
			_ = action ?? throw new ArgumentNullException(nameof(action));

#if ANDROID || IOS
			return DispatchDelayedImplementation(delay, action);
#else
			return false;
#endif
		}

		/// <inheritdoc/>
		public IDispatcherTimer CreateTimer()
		{
#if ANDROID || IOS
			return CreateTimerImplementation();
#else
			return null;
#endif
		}

		//private partial bool IsDispatchRequiredImplementation() => false;

		//private partial bool DispatchImplementation(Action action) => false;

		//private partial bool DispatchDelayedImplementation(TimeSpan delay, Action action) => false;

		//private partial IDispatcherTimer CreateTimerImplementation() => null;
	}
}
