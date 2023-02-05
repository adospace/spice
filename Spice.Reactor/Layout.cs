using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Reactor;

public partial interface ILayout : IView
{

}

public abstract partial class Layout<T> : View<T>, ILayout where T : Spice.Layout, new()
{
	protected Layout()
	{
	}

	protected Layout(Action<T?> componentRefAction) : base(componentRefAction)
	{
	}

	protected override void OnAddChild(VisualNode widget, object childControl)
	{
		Validate.EnsureNotNull(NativeControl);

		if (childControl is Spice.View control)
		{
			NativeControl.Children.Insert(widget.ChildIndex, control);
		}

		base.OnAddChild(widget, childControl);
	}

	protected override void OnRemoveChild(VisualNode widget, object childControl)
	{
		Validate.EnsureNotNull(NativeControl);

		if (childControl is Spice.View control)
		{
			NativeControl.Children.Remove(control);
		}

		base.OnRemoveChild(widget, childControl);
	}
}

public static partial class LayoutExtensions
{

}
