﻿using Android.Content;
namespace Spice;

public partial class StackView
{
	public static implicit operator LinearLayout(StackView stackView) => stackView.NativeView;

	public StackView() : base(Platform.Context!,
		c => new LinearLayout(c)
		{
			// TODO: reduce JNI calls
			Orientation = Android.Widget.Orientation.Vertical
		})
	{ }

	public StackView(Context context, Func<Context, Android.Views.View> creator) : base(context, creator) { }

	protected override Android.Views.ViewGroup.LayoutParams CreateLayoutParameters() =>
		new LinearLayout.LayoutParams(Android.Views.ViewGroup.LayoutParams.WrapContent, Android.Views.ViewGroup.LayoutParams.WrapContent);

	public new LinearLayout NativeView => (LinearLayout)_nativeView.Value;

	partial void OnOrientationChanging(Orientation value)
	{
		switch (value)
		{
			case Orientation.Vertical:
				NativeView.Orientation = Android.Widget.Orientation.Vertical;
				break;
			case Orientation.Horizontal:
				NativeView.Orientation = Android.Widget.Orientation.Horizontal;
				break;
			default:
				throw new NotSupportedException($"{nameof(Orientation)} value '{value}' not supported!");
		}
	}
}