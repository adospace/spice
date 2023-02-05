using UIKit;
using Foundation;

namespace Spice.Scenarios;

[Register("AppDelegate")]
public class AppDelegate : SpiceAppDelegate
{
	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		base.FinishedLaunching(application, launchOptions);

		ArgumentNullException.ThrowIfNull(Window);

		var vc = new UIViewController();
		var app = new AppBuilder().Build();
		vc.View!.AddSubview(app);
		Window.RootViewController = vc;
		Window.MakeKeyAndVisible();

		// Uncomment to debug UI layout
		//vc.View.DumpHierarchy();

		return true;
	}
}