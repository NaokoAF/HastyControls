using HastyControls.Core;
using HastyControls.Core.Patches;
using UnityEngine;
using UnityEngine.InputSystem;
using ILogger = HastyControls.Core.ILogger;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls;

internal class HastyControlsMod : MonoBehaviour
{
	public ILogger Logger => logger!;
	public HasteEvents Events => events!;
	public GyroPauser GyroPauser => gyroPauser!;
	public SDLManager SDL => sdl!;
	public SteamInputManager SteamInputManager => steamInputManager!;
	public ControllerManager ControllerManager => controllerManager!;
	public HasteRumble Rumble => rumble!;

	private ILogger? logger;
	private SDLManager? sdl;
	private HasteEvents? events;
	private GyroPauser? gyroPauser;
	private SteamInputManager? steamInputManager;
	private ControllerManager? controllerManager;
	private HasteRumble? rumble;
	private readonly List<IHastyPatch> patches = new();

	public void Initialize(ILogger logger, SDLManager sdl)
	{
		this.logger = logger;
		this.sdl = sdl;
		events = new();
		gyroPauser = new(events);
		steamInputManager = new();
		controllerManager = new(sdl, steamInputManager);
		rumble = new(events, controllerManager);

		// logging
		sdl.ControllerAdded += controller => Logger.Msg($"Controller added - {GetControllerPrintName(controller)}");
		sdl.ControllerRemoved += controller => Logger.Msg($"Controller removed - {GetControllerPrintName(controller)}");
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

		// apply patches
		foreach (Type type in typeof(IHastyPatch).Assembly.GetTypes())
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

		controllerManager.GyroButtonDown = GyroButtonAction?.IsPressed() ?? false;
		controllerManager.GyroButtonMode = GetSetting<GyroButtonModeSetting>().Value;
		controllerManager.GyroCalibrateButtonDown = (GyroCalibrateAction?.IsPressed() ?? false) && EscapeMenu.IsOpen;
		controllerManager.GyroPaused = gyroPauser.IsGyroPaused;

		if (GetSetting<GyroUseTouchpadAsModifier>().Value && controllerManager.ActiveController != null)
			controllerManager.GyroButtonDown |= controllerManager.ActiveController.IsAnyTouchpadDown();

		steamInputManager!.Update(Time.unscaledDeltaTime);
		controllerManager.Update(Time.unscaledDeltaTime);
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