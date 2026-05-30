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
	internal static SteamInputManager? SteamInputManager;
	internal static ControllerManager? ControllerManager;
	internal static HasteRumble? Rumble;

	public static void Initialize(SDL sdl)
	{
		SDL = new(sdl);
		SteamInputManager = new();
		ControllerManager = new(SDL, SteamInputManager);
		Rumble = new(Events, ControllerManager);

		// add logging
		SDL.ControllerAdded += controller => Logger.Msg($"Controller {controller.Id} added - {controller.Name} (Gyro: {controller.HasGyro})");
		SDL.ControllerRemoved += controller => Logger.Msg($"Controller {controller.Id} removed - {controller.Name}");
		ControllerManager.GyroBiasCalibrated += (controller, bias) => Logger.Msg($"Controller {controller.Id} calibrated - {controller.Name} (Bias: {bias})");

		// update config
		UpdateConfig();
		GetSetting<GamepadDeadzonesSetting>().Applied += (_) => UpdateConfig();
		
		// some controllers can disable their gyro at the firmware level
		// other programs (notably steam) can interact with this and stop us from reading gyro data
		// as a workaround, we toggle gyro off and back on when pausing or alt-tabbing
		Events.EscapeMenuToggled += _ => ControllerManager.ForceEnableGyro();
		Application.focusChanged += _ => ControllerManager.ForceEnableGyro();

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
		Rumble!.Update(Time.unscaledDeltaTime);
		GyroPauser.Update();

		ControllerManager.GyroButtonDown = HastySettings.GyroButtonAction?.IsPressed() ?? false;
		ControllerManager.GyroButtonMode = GetSetting<GyroButtonModeSetting>().Value;
		ControllerManager.GyroCalibrateButtonDown = (HastySettings.GyroCalibrateAction?.IsPressed() ?? false) && EscapeMenu.IsOpen;
		ControllerManager.GyroPaused = GyroPauser.IsGyroPaused;

		if (GetSetting<GyroUseTouchpadAsModifier>().Value && ControllerManager.ActiveController != null)
			ControllerManager.GyroButtonDown |= ControllerManager.ActiveController.IsAnyTouchpadDown();

		SteamInputManager!.Update(Time.unscaledDeltaTime);
		ControllerManager.Update(Time.unscaledDeltaTime);
	}

	public static void Deinitialize()
	{
		SDL!.Quit();
		SteamInputManager!.Dispose();
	}

	static void UpdateConfig()
	{
		InputSystem.settings.defaultDeadzoneMin = GetSetting<GamepadDeadzonesSetting>().Value;
		InputSystem.settings.defaultDeadzoneMax = 1f;
	}
}