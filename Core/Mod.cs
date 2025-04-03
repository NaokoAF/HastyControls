using HastyControls.SDL3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace HastyControls.Core;

public static class Mod
{
	internal static ILogger Logger = new UnityDebugLogger();
	internal static Config Config = new();
	internal static HasteEvents Events = new();
	internal static GyroPauser GyroPauser = new(Config, Events);
	internal static SDLManager? SDL;
	internal static ControllerManager? ControllerManager;
	internal static HasteRumble? Rumble;

	public static void Initialize(SDL sdl)
	{
		SDL = new(sdl);
		ControllerManager = new(SDL, Config);
		Rumble = new(Events, ControllerManager, Config);

		// add logging
		SDL.ControllerAdded += controller => Logger.Msg($"Controller {controller.Id} added - {controller.Name} (Gyro: {controller.HasGyro})");
		SDL.ControllerRemoved += controller => Logger.Msg($"Controller {controller.Id} removed - {controller.Name}");
		ControllerManager.GyroBiasCalibrated += (controller, bias) => Logger.Msg($"Controller {controller.Id} calibrated - {controller.Name} (Bias: {bias})");
		ControllerManager.ActiveControllerChanged += controller =>
		{
			if (controller != null)
				Logger.Msg($"Set active controller to {controller.Id} - {controller.Name}");
			else
				Logger.Msg("No controller active!");
		};

		// initialize SDL
		Logger.Msg($"SDL {SDL.Version.Major}.{SDL.Version.Minor}.{SDL.Version.Micro} ({SDL.Revision})");
		if (!SDL.Init())
		{
			Logger.Msg($"Failed to initialize SDL: {SDL.CurrentError}");
		}
	}

	public static void UpdateConfig()
	{
		InputSystem.settings.defaultDeadzoneMin = Math.Clamp(Config.SticksDeadzones, 0.01f, 1f);
		InputSystem.settings.defaultDeadzoneMax = 1f;

		ControllerManager!.UpdateConfig(Config);
	}

	public static void Update()
	{
		SDL!.Poll();

		ControllerManager!.GyroPaused = GyroPauser.Update();
		ControllerManager!.Update(Time.unscaledDeltaTime);
	}

	public static void Deinitialize()
	{
		SDL!.Quit();
	}
}