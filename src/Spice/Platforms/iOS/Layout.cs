namespace Spice;

public abstract partial class Layout
{
	/// <inheritdoc />
	/// <param name="creator">Subclasses can pass in a Func to create a UIView</param>
	protected Layout(Func<View, UIView> creator) : base(creator) { }
}