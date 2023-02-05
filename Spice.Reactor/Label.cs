using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Graphics;

namespace Spice.Reactor;

public partial interface ILabel : ILayout
{
	PropertyValue<string>? Text { get; set; }
	PropertyValue<Color>? TextColor { get; set; }
}

public partial class Label<T> : View<T>, ILabel where T : Spice.Label, new()
{
	public Label()
	{
	}

	public Label(Action<T?> componentRefAction) : base(componentRefAction)
	{
	}

	PropertyValue<string>? ILabel.Text { get; set; }
	PropertyValue<Color>? ILabel.TextColor { get; set; }

	protected override void OnUpdate()
	{
		OnBeginUpdate();
		Validate.EnsureNotNull(NativeControl);
		var thisAsILabel = (ILabel)this;

		if (thisAsILabel.Text != null)
		{
			NativeControl.Text = thisAsILabel.Text.Value ?? string.Empty;
		}
		if (thisAsILabel.TextColor != null)
		{
			NativeControl.TextColor = thisAsILabel.TextColor.Value;
		}

		//SetPropertyValue(NativeControl, Microsoft.Maui.Controls.LabelLayout.OrientationProperty, thisAsILabelLayout.Orientation);

		base.OnUpdate();
		OnEndUpdate();
	}

	partial void OnBeginUpdate();
	partial void OnEndUpdate();
}

public partial class Label : Label<Spice.Label>
{
	public Label()
	{
	}

	public Label(Action<Spice.Label?> componentRefAction) : base(componentRefAction)
	{
	}
}

public static partial class LabelExtensions
{

	public static T Text<T>(this T label, string text)
		where T : ILabel
	{
		label.Text = new PropertyValue<string>(text);
		return label;
	}

	public static T Text<T>(this T label, Func<string> textFunc)
		where T : ILabel
	{
		label.Text = new PropertyValue<string>(textFunc);
		return label;
	}
}