using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Reactor;

public interface IHostElement
{
	IHostElement Run();

	void Stop();

	void RequestAnimationFrame(VisualNode visualNode);
}