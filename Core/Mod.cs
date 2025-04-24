using HastyControls.Core.Settings;
using HastyControls.SDL3;
using UnityEngine;
using UnityEngine.InputSystem;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

public static class Mod
{
	internal static ILogger Logger = new UnityDebugLogger();
	internal static HasteEvents Events = new();
	internal static GyroPauser GyroPauser = new(Events);
	internal static SDLManager? SDL;
	internal static ControllerManager? ControllerManager;
	internal static HasteRumble? Rumble;

	public static void Initialize(SDL sdl)
	{
		SDL = new(sdl);
		ControllerManager = new(SDL);
		Rumble = new(Events, ControllerManager);

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

		// update config
		UpdateConfig();
		GetSetting<GamepadDeadzonesSetting>().Applied += (_) => UpdateConfig();

		// initialize SDL
		Logger.Msg($"SDL {SDL.Version.Major}.{SDL.Version.Minor}.{SDL.Version.Micro} ({SDL.Revision})");
		if (!SDL.Init())
		{
			Logger.Msg($"Failed to initialize SDL: {SDL.CurrentError}");
		}
	}

	public static void Update()
	{
		ControllerManager!.PrePoll();
		SDL!.Poll();

		ControllerManager!.GyroPaused = GyroPauser.Update();
		ControllerManager!.Update(Time.unscaledDeltaTime);
	}

	public static void Deinitialize()
	{
		SDL!.Quit();
	}

	static void UpdateConfig()
	{
		InputSystem.settings.defaultDeadzoneMin = GetSetting<GamepadDeadzonesSetting>().Value;
		InputSystem.settings.defaultDeadzoneMax = 1f;
	}
}