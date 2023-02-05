﻿namespace Spice.Scenarios;

/// <summary>
/// Most basic scenario
/// NOTE: my color choices are suspect! 🌈🤢
/// </summary>
class HelloWorldScenario : Stack
{
	public HelloWorldScenario()
	{
		int count = 0;

		var label = new Label
		{
			Text = "Hello, Spice 🌶",
			TextColor = Colors.Red
		};

		var button = new Button
		{
			Text = "Click Me",
			Clicked = _ => label.Text = $"Times: {++count}"
		};

		Add(new Image { Source = "spice" });
		Add(label);
		Add(button);
	}
}
