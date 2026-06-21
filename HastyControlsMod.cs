using System.Reflection;
using HastyControls.Core;
using HastyControls.Core.Patches;
using HastyControls.Core.Settings;
using HastyControls.SDL3;
using UnityEngine;
using UnityEngine.InputSystem;
using ILogger = HastyControls.Core.ILogger;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls;

internal class HastyControlsMod : MonoBehaviour
{
	public ILogger Logger => logger!;
	public HasteEvents Events  => events!;
	public GyroPauser GyroPauser => gyroPauser!;
	public SDLManager SDL => sdl!;
	public SteamInputManager SteamInputManager => steamInputManager!;
	public ControllerManager ControllerManager => controllerManager!;
	public HasteRumble Rumble => rumble!;
	
	private ILogger logger = new UnityDebugLogger();
	private HasteEvents events = new();
	private GyroPauser? gyroPauser;
	private SDLManager? sdl;
	private SteamInputManager? steamInputManager;
	private ControllerManager? controllerManager;
	private HasteRumble? rumble;

	private readonly List<IHastyPatch> patches = new();

	private void Awake()
	{
		logger.Msg($"Hello from {ModInfo.Name} {ModInfo.Version}");

		// Landfall's modding library loads all DLLs in the workshop folder, but when it tries to load SDL an error happens
		// so we need to change the extension to something else to avoid that error
		// I'm sure there's a better way around this, but this is all I could come up with
		string sdlPath = Path.Combine(GetAssemblyDirectory(), "SDL3.dll.assetbundle");
		logger.Msg($"Loading SDL from {sdlPath}");
		sdl = new(new SDL(sdlPath));
		
		Logger.Msg($"SDL Version: {sdl.Revision}");

		// initialize
		gyroPauser = new(events);
		steamInputManager = new();
		controllerManager = new(sdl, steamInputManager);
		rumble = new(events, controllerManager);
		
		// logging
		sdl.ControllerAdded += controller =>
			Logger.Msg($"Controller added - {GetControllerPrintName(controller)}");
		sdl.ControllerRemoved += controller =>
			Logger.Msg($"Controller removed - {GetControllerPrintName(controller)}");
		ControllerManager.GyroBiasCalibrated += (controller, bias) =>
			Logger.Msg($"Controller calibrated - {GetControllerPrintName(controller)} (Bias: {bias})");
		ControllerManager.GyroOrientationChanged += (controller, orientation) =>
			Logger.Msg($"Controller orientation changed - {GetControllerPrintName(controller)} (Orientation: {orientation})");

		// update config
		UpdateConfig();
		GetSetting<GamepadDeadzonesSetting>().Applied += (_) => UpdateConfig();

		// some controllers can disable their gyro at the firmware level
		// other programs (notably steam) can interact with this and stop us from reading gyro data
		// as a workaround, we toggle gyro off and back on when pausing or alt-tabbing
		Events.EscapeMenuToggled += _ => ControllerManager.ForceEnableGyro();
		Application.focusChanged += _ => ControllerManager.ForceEnableGyro();

		// initialize SDL
		if (!sdl.Init())
		{
			Logger.Msg($"Failed to initialize SDL: {sdl.CurrentError}");
		}
		
		// apply patches
		foreach(Type type in typeof(IHastyPatch).Assembly.GetTypes())
		{
			if (type == typeof(IHastyPatch) || !typeof(IHastyPatch).IsAssignableFrom(type)) continue;

			logger.Msg($"Applying patch {type.Name}");

			try
			{
				IHastyPatch patch = (IHastyPatch)Activator.CreateInstance(type)!;
				patch.Patch(this);
				
				// keep patches in a list so C# doesn't garbage collect them
				patches.Add(patch);
			}
			catch (Exception e)
			{
				logger.Error($"Failed to load patch {type.Name}: {e}");
			}
		}
	}

	private void Update()
	{
		controllerManager!.PrePoll();
		sdl!.Poll();
		rumble!.Update(Time.unscaledDeltaTime);
		gyroPauser!.Update();

		controllerManager.GyroButtonDown = HastySettings.GyroButtonAction?.IsPressed() ?? false;
		controllerManager.GyroButtonMode = GetSetting<GyroButtonModeSetting>().Value;
		controllerManager.GyroCalibrateButtonDown = (HastySettings.GyroCalibrateAction?.IsPressed() ?? false) && EscapeMenu.IsOpen;
		controllerManager.GyroPaused = gyroPauser.IsGyroPaused;

		if (GetSetting<GyroUseTouchpadAsModifier>().Value && controllerManager.ActiveController != null)
			controllerManager.GyroButtonDown |= controllerManager.ActiveController.IsAnyTouchpadDown();

		steamInputManager!.Update(Time.unscaledDeltaTime);
		controllerManager.Update(Time.unscaledDeltaTime);
	}
	
	// janky solution to find where the workshop folder is
	private static string GetAssemblyDirectory()
	{
		string codeBase = Assembly.GetExecutingAssembly().CodeBase;
		UriBuilder uri = new UriBuilder(codeBase);
		string path = Uri.UnescapeDataString(uri.Path);
		return Path.GetDirectoryName(path)!;
	}

	private static void UpdateConfig()
	{
		InputSystem.settings.defaultDeadzoneMin = GetSetting<GamepadDeadzonesSetting>().Value;
		InputSystem.settings.defaultDeadzoneMax = 1f;
	}
	
	public string GetControllerPrintName(SDLController controller)
	{
		string type = controller.GamepadType.ToString().Replace("SDL_GAMEPAD_TYPE_", "");
		
		string str = $"{controller.VendorId:x4}:{controller.ProductId:x4} {controller.Name} [{type}]";
		if (controller.SteamHandle != 0) return $"{str} [Steam Input]";
		if (controller.HasGyro) return $"{str} [No Gyro]";
		return str;
	}
}