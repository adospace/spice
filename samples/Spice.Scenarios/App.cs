namespace Spice.Scenarios;

public class App : Application
{
	public App()
	{
		BackgroundColor = Colors.CornflowerBlue;

		// Comment/uncomment to try different scenarios/samples
		Main = new HelloWorldScenario();

		
		//Main = new GhostButtonScenario();
	}
}

public class AppBuilder
{
	public Application Build()
	{
		var app = new Spice.Reactor.ReactorApplication<Scenarios.HelloWorldReactor>();

		app.Run();

		return app;
	}
}