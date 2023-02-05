using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spice.Reactor;

namespace Spice.Scenarios.Scenarios;

class HelloWorldReactor : Component
{
	public override VisualNode Render()
		=> new Reactor.Stack
		{
			new Reactor.Label()
				.Text("Hello World!")				
		};
}
