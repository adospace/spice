﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Spice.Core.Dispatching;
using Spice.Reactor.HotReload;

namespace Spice.Reactor;

internal abstract class ReactorApplicationHost : VisualNode, IHostElement, IVisualNode
{
	protected readonly ReactorApplication _application;

	protected ReactorApplicationHost(ReactorApplication application, bool enableHotReload)
	{
		_instance = this;

		_application = application ?? throw new ArgumentNullException(nameof(application));

		if (enableHotReload)
		{
			ComponentLoader = new RemoteComponentLoader();
			ComponentLoader.AssemblyChanged += (s, e) => OnComponentAssemblyChanged();
		}
		else
		{
			ComponentLoader = new LocalComponentLoader();
		}

	}

	private static ReactorApplicationHost? _instance;
	public static ReactorApplicationHost Instance => _instance ?? throw new InvalidOperationException();

	internal IComponentLoader ComponentLoader { get; }

	public Action<UnhandledExceptionEventArgs>? UnhandledException { get; set; }

	internal void FireUnhandledExpectionEvent(Exception ex)
	{
		UnhandledException?.Invoke(new UnhandledExceptionEventArgs(ex, false));
		System.Diagnostics.Debug.WriteLine(ex);
	}

	public abstract IHostElement Run();

	public abstract void Stop();

	protected virtual void OnComponentAssemblyChanged()
	{ }

	public ReactorApplicationHost OnUnhandledException(Action<UnhandledExceptionEventArgs> action)
	{
		UnhandledException = action;
		return this;
	}

	public abstract void RequestAnimationFrame(VisualNode visualNode);

	//public INavigation? Navigation => _application.MainPage?.Navigation;

	public IServiceProvider Services => _application.Services;

	public View? ContainerPage => _application?.Main;

	IHostElement? IVisualNode.GetPageHost()
	{
		return this;
	}

	//Microsoft.Maui.Controls.Page? IVisualNode.GetContainerPage()
	//{
	//	return ContainerPage;
	//}


}

internal class ReactorApplicationHost<T> : ReactorApplicationHost where T : Component, new()
{
	private Component? _rootComponent;
	private bool _sleeping = false;
	private bool _started = false;
	private readonly LinkedList<VisualNode> _listOfVisualsToAnimate = new();

	internal ReactorApplicationHost(ReactorApplication<T> application, bool enableHotReload)
		: base(application, enableHotReload)
	{
	}

	protected sealed override void OnAddChild(VisualNode widget, object nativeControl)
	{
		_application.Main = nativeControl is View page
			? page
			:            
			throw new NotSupportedException($"Invalid root component ({nativeControl.GetType()}): must be a View");
	}

	protected sealed override void OnRemoveChild(VisualNode widget, object nativeControl)
	{
		//MainPage can't be set to null!
		//_application.MainPage = null;
	}

	public void Pause()
	{
		_sleeping = true;
	}

	public void Resume()
	{
		_sleeping = false;
	}

	public override IHostElement Run()
	{
		if (!_started)
		{
			_started = true;
			_rootComponent ??= new T();
			OnLayout();
			ComponentLoader.Run();
		}

		return this;
	}

	protected override void OnComponentAssemblyChanged()
	{
		try
		{
			var newComponent = ComponentLoader.LoadComponent<T>();
			if (newComponent != null &&
				_rootComponent != newComponent)
			{
				_rootComponent = newComponent;

				Invalidate();
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"Unable to hot relead component {typeof(T).FullName}: type not found in received assembly");
			}
		}
		catch (Exception ex)
		{
			FireUnhandledExpectionEvent(ex);
		}

	}

	public override void Stop()
	{
		if (_started)
		{
			_started = false;
			ComponentLoader.Stop();
		}
	}

	protected internal override void OnLayoutCycleRequested()
	{
		if (_started && !_sleeping)
		{
			Dispatcher.GetForCurrentThread()?.Dispatch(OnLayout);
		}
		base.OnLayoutCycleRequested();
	}

	private void OnLayout()
	{
		try
		{
			Layout();
			SetupAnimationTimer();
		}
		catch (Exception ex)
		{
			FireUnhandledExpectionEvent(ex);
		}
	}

	protected override IEnumerable<VisualNode?> RenderChildren()
	{
		yield return _rootComponent;
	}

	public override void RequestAnimationFrame(VisualNode visualNode)
	{
		_listOfVisualsToAnimate.AddFirst(visualNode);
	}

	private void SetupAnimationTimer()
	{
		if (_listOfVisualsToAnimate.Count > 0)
		{
			Dispatcher.GetForCurrentThread()?.Dispatch(AnimationCallback);
		}
	}

	private void AnimationCallback()
	{
		if (!_started || _sleeping)
		{
			return;
		}
		DateTime now = DateTime.Now;
		if (AnimateVisuals())
		{
			//System.Diagnostics.Debug.WriteLine($"{(DateTime.Now - now).TotalMilliseconds}");
			var elapsedMilliseconds = (DateTime.Now - now).TotalMilliseconds;
			if (elapsedMilliseconds > 16)
			{
				System.Diagnostics.Debug.WriteLine("FPS WARNING");
				Dispatcher.GetForCurrentThread()?.Dispatch(AnimationCallback);
			}
			else
			{
				Dispatcher.GetForCurrentThread()?.DispatchDelayed(TimeSpan.FromMilliseconds(16 - elapsedMilliseconds), AnimationCallback);
			}
		}
	}

	private bool AnimateVisuals()
	{
		if (_listOfVisualsToAnimate.Count == 0)
			return false;

		bool animated = false;
		LinkedListNode<VisualNode>? nodeToAnimate = _listOfVisualsToAnimate.First;
		while (nodeToAnimate != null)
		{
			var nextNode = nodeToAnimate.Next;

			if (nodeToAnimate.Value.Animate())
			{
				animated = true;
			}
			else
			{
				_listOfVisualsToAnimate.Remove(nodeToAnimate);
			}

			nodeToAnimate = nextNode;
		}

		return animated;
	}
}

public abstract class ReactorApplication : Application
{
	protected ReactorApplication(/*IServiceProvider sp*/)
	{
		//Services = sp;
	}

	internal static bool HotReloadEnabled { get; set; }

	public IServiceProvider Services { get; }
}

public class ReactorApplication<T> : ReactorApplication where T : Component, new()
{
	private ReactorApplicationHost<T>? _host;

	public ReactorApplication(/*IServiceProvider sp*/)
		: base(/*sp*/)
	{ 
	
	}

	public void Run()
	{
		_host ??= new ReactorApplicationHost<T>(this, HotReloadEnabled);
		_host.Run();
	}

	//protected override Window CreateWindow(IActivationState? activationState)
	//{
	//	_host ??= new ReactorApplicationHost<T>(this, HotReloadEnabled);
	//	_host.Run();
	//	return base.CreateWindow(activationState);
	//}

	//public override void CloseWindow(Window window)
	//{
	//	base.CloseWindow(window);
	//}

	//protected override void OnStart()
	//{
	//	_host?.Run();
	//	base.OnStart();
	//}

	//protected override void OnResume()
	//{
	//	_host?.Resume();
	//	base.OnResume();
	//}

	//protected override void OnSleep()
	//{
	//	_host?.Pause();
	//	base.OnSleep();
	//}
	//protected override void CleanUp()
	//{
	//	_host?.Stop();
	//	base.CleanUp();
	//}
}

//public static class MauiAppBuilderExtensions
//{
//	public static MauiAppBuilder UseMauiReactorApp<TComponent>(this MauiAppBuilder appBuilder, Action<Application>? configureApplication = null) where TComponent : Component, new()
//		=> appBuilder.UseMauiApp(sp =>
//		{
//			var app = new ReactorApplication<TComponent>(sp);
//			configureApplication?.Invoke(app);
//			return app;
//		});

//	public static MauiAppBuilder EnableMauiReactorHotReload(this MauiAppBuilder appBuilder)
//	{
//		ReactorApplication.HotReloadEnabled = true;
//		return appBuilder;
//	}
//}

public static class ApplicationExtensions
{
	//public static Application AddResource(this Application application, string resourceName, Assembly? containerAssembly = null)
	//{
	//	var resourceDictionary = new ResourceDictionary();
	//	resourceDictionary.SetAndLoadSource(
	//		new Uri(resourceName, UriKind.Relative),
	//		resourceName,
	//		containerAssembly ?? Assembly.GetCallingAssembly(),
	//		null);

	//	application.Resources.Add(resourceDictionary);

	//	return application;
	//}

	//public static Application SetWindowsSpecificAssectDirectory(this Application application, string directoryName)
	//{
	//	Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific.Application.SetImageDirectory(application, directoryName);

	//	return application;
	//}
}
