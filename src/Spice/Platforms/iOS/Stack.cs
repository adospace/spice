namespace Spice;

public partial class Stack
{
	/// <summary>
	/// Returns stackView.NativeView
	/// </summary>
	/// <param name="stackView">The Spice.StackView</param>
	public static implicit operator UIStackView(Stack stackView) => stackView.NativeView;

	/// <summary>
	/// A parent view for laying out child controls in a row. Defaults to Orientation=Vertical.
	/// Android -> Android.Widget.LinearLayout
	/// iOS -> UIKit.UIStackView
	/// </summary>
	public Stack() : this(v => new SpiceStackView((Stack)v) { AutoresizingMask = UIViewAutoresizing.None, Alignment = UIStackViewAlignment.Center, Axis = UILayoutConstraintAxis.Vertical }) { }

	/// <inheritdoc />
	/// <param name="frame">Pass the underlying view a frame</param>
	public Stack(CGRect frame) : this(v => new SpiceStackView((Stack)v, frame) { AutoresizingMask = UIViewAutoresizing.None, Alignment = UIStackViewAlignment.Center, Axis = UILayoutConstraintAxis.Vertical }) { }

	/// <inheritdoc />
	/// <param name="creator">Subclasses can pass in a Func to create a UIView</param>
	protected Stack(Func<View, UIView> creator) : base(creator) { }

	/// <summary>
	/// The underlying UIStackView
	/// </summary>
	public new UIStackView NativeView => (UIStackView)_nativeView.Value;

	partial void OnOrientationChanging(Orientation value)
	{
		switch (value)
		{
			case Orientation.Vertical:
				NativeView.Axis = UILayoutConstraintAxis.Vertical;
				break;
			case Orientation.Horizontal:
				NativeView.Axis = UILayoutConstraintAxis.Horizontal;
				break;
			default:
				throw new NotSupportedException($"{nameof(Orientation)} value '{value}' not supported!");
		}
	}

	/// <inheritdoc />
	protected override void AddSubview(View view)
	{
		NativeView.AddArrangedSubview(view);
		view.UpdateAlign();
	}

	class SpiceStackView : UIStackView
	{
		readonly Stack _parent;

		public SpiceStackView(Stack parent) => _parent = parent;

		public SpiceStackView(Stack parent, CGRect frame) : base(frame) => _parent = parent;

		public override CGSize SizeThatFits(CGSize size)
		{
			var measured = new CGSize();
			foreach (var child in Subviews)
			{
				var childSize = child.SizeThatFits(size);
				if (_parent.Orientation == Orientation.Vertical)
				{
					if (childSize.Width > measured.Width)
						measured.Width = childSize.Width;
					measured.Height += childSize.Height;
				}
				else if (_parent.Orientation == Orientation.Horizontal)
				{
					measured.Width += childSize.Width;
					if (childSize.Height > measured.Height)
						measured.Height = childSize.Height;
				}
			}
			return measured;
		}
	}
}