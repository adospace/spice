using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Reactor;

public partial interface IView : IVisualNode
{

}

public abstract partial class View<T> : VisualNode<T>, IView, IEnumerable where T : Spice.View, new()
{
	protected View()
	{
	}

	protected View(Action<T?> componentRefAction) : base(componentRefAction)
	{
	}

	protected readonly List<VisualNode> _internalChildren = new();

	protected override IEnumerable<VisualNode> RenderChildren()
	{
		return _internalChildren;
	}


	public IEnumerator<VisualNode> GetEnumerator()
	{
		return _internalChildren.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _internalChildren.GetEnumerator();
	}

	public void Add(params VisualNode?[]? nodes)
	{
		if (nodes is null)
		{
			return;
			//throw new ArgumentNullException(nameof(nodes));
		}

		foreach (var node in nodes)
		{
			if (node != null)
			{
				OnChildAdd(node);
			}
		}
	}

	public void Add(object? genericNode)
	{
		if (genericNode == null)
		{
			return;
		}

		if (genericNode is VisualNode visualNode)
		{
			Add(visualNode);
		}
		else if (genericNode is IEnumerable nodes)
		{
			foreach (var node in nodes.Cast<VisualNode>())
			{
				Add(node);
			}
		}
		else
		{
			throw new NotSupportedException($"Unable to add value of type '{genericNode.GetType()}' under {typeof(T)}");
		}
	}

	protected virtual void OnChildAdd(VisualNode node)
	{
		_internalChildren.Add(node);
		OnChildAdded(node);
	}

	protected virtual void OnChildAdded(VisualNode node)
	{ }

}

public static partial class ViewExtensions
{

}
