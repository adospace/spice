using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Reactor;

public partial interface IStack : ILayout
{
	PropertyValue<Spice.Orientation>? Orientation { get; set; }
}

public partial class Stack<T> : Layout<T>, IStack where T : Spice.Stack, new()
{
	public Stack()
	{
	}

	public Stack(Action<T?> componentRefAction) : base(componentRefAction)
	{
	}

	PropertyValue<Spice.Orientation>? IStack.Orientation { get; set; }

	protected override void OnUpdate()
	{
		OnBeginUpdate();
		Validate.EnsureNotNull(NativeControl);
		var thisAsIStackLayout = (IStack)this;

		if (thisAsIStackLayout.Orientation != null)
		{
			NativeControl.Orientation = thisAsIStackLayout.Orientation.Value;
		}

		//SetPropertyValue(NativeControl, Microsoft.Maui.Controls.StackLayout.OrientationProperty, thisAsIStackLayout.Orientation);

		base.OnUpdate();
		OnEndUpdate();
	}

	partial void OnBeginUpdate();
	partial void OnEndUpdate();
}

public partial class Stack : Stack<Spice.Stack>
{
	public Stack()
	{
	}

	public Stack(Action<Spice.Stack?> componentRefAction) : base(componentRefAction)
	{
	}
}

public static partial class StackExtensions
{

}