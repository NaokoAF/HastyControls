using System.Runtime.InteropServices;

namespace HastyControls.SDL3;

// i can't put into words how gross and terrible and bad this code is
// but i couldn't get it to work any other way, and i tried for hours
// if someone more knowledgeable than me can figure out how to load the SDL3 DLL into Haste, feel free to help
public unsafe partial class SDL : IDisposable
{
	private nint library;

	public SDL(string path)
	{
		library = LoadLibrary(path);

		// SDL_init
		Init = GetFunction<SDL_Init>();
		Quit = GetFunction<SDL_Quit>();

		// SDL_stdinc
		malloc = GetFunction<SDL_malloc>();
		calloc = GetFunction<SDL_calloc>();
		realloc = GetFunction<SDL_realloc>();
		free = GetFunction<SDL_free>();

		// SDL_error
		GetError = GetFunction<SDL_GetError>();
		ClearError = GetFunction<SDL_ClearError>();

		// SDL_version
		GetVersion = GetFunction<SDL_GetVersion>();
		GetRevision = GetFunction<SDL_GetRevision>();

		// SDL_events
		PollEvent = GetFunction<SDL_PollEvent>();

		// SDL_joystick
		UpdateJoysticks = GetFunction<SDL_UpdateJoysticks>();
		GetJoystickVendorForID = GetFunction<SDL_GetJoystickVendorForID>();
		GetJoystickProductForID = GetFunction<SDL_GetJoystickProductForID>();

		// SDL_gamepad
		OpenGamepad = GetFunction<SDL_OpenGamepad>();
		GetGamepadID = GetFunction<SDL_GetGamepadID>();
		GetGamepadJoystick = GetFunction<SDL_GetGamepadJoystick>();
		GetGamepadName = GetFunction<SDL_GetGamepadName>();
		GetGamepadType = GetFunction<SDL_GetGamepadType>();
		SetGamepadEventsEnabled = GetFunction<SDL_SetGamepadEventsEnabled>();
		GamepadEventsEnabled = GetFunction<SDL_GamepadEventsEnabled>();
		UpdateGamepads = GetFunction<SDL_UpdateGamepads>();
		GetNumGamepadTouchpads = GetFunction<SDL_GetNumGamepadTouchpads>();
		GetNumGamepadTouchpadFingers = GetFunction<SDL_GetNumGamepadTouchpadFingers>();
		GetGamepadTouchpadFinger = GetFunction<SDL_GetGamepadTouchpadFinger>();
		GamepadHasSensor = GetFunction<SDL_GamepadHasSensor>();
		SetGamepadSensorEnabled = GetFunction<SDL_SetGamepadSensorEnabled>();
		GamepadSensorEnabled = GetFunction<SDL_GamepadSensorEnabled>();
		GetGamepadSensorDataRate = GetFunction<SDL_GetGamepadSensorDataRate>();
		GetGamepadSensorData = GetFunction<SDL_GetGamepadSensorData>();
		RumbleGamepad = GetFunction<SDL_RumbleGamepad>();
		RumbleGamepadTriggers = GetFunction<SDL_RumbleGamepadTriggers>();
		SetGamepadLED = GetFunction<SDL_SetGamepadLED>();
		CloseGamepad = GetFunction<SDL_CloseGamepad>();
		GetGamepadSteamHandle = GetFunction<SDL_GetGamepadSteamHandle>();

		// SDL_sensor
		GetSensors = GetFunction<SDL_GetSensors>();
		GetSensorNameForID = GetFunction<SDL_GetSensorNameForID>();
		OpenSensor = GetFunction<SDL_OpenSensor>();
		GetSensorFromID = GetFunction<SDL_GetSensorFromID>();
		GetSensorName = GetFunction<SDL_GetSensorName>();
		GetSensorType = GetFunction<SDL_GetSensorType>();
		CloseSensor = GetFunction<SDL_CloseSensor>();
		UpdateSensors = GetFunction<SDL_UpdateSensors>();
	}

	public void Dispose()
	{
		FreeLibrary(library);
	}

	public string? PtrToStringUTF8(byte* ptr, bool free = false)
	{
		string? s = Marshal.PtrToStringUTF8((nint)ptr);
		if (free) this.free((nint)ptr);
		return s;
	}

	private T GetFunction<T>() where T : Delegate
	{
		nint pointer = GetProcAddress(library, typeof(T).Name);
		return (T)Marshal.GetDelegateForFunctionPointer(pointer, typeof(T));
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern nint LoadLibrary(string lpLibFileName);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern void FreeLibrary(nint hLibModule);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern nint GetProcAddress(nint hModule, string lpProcName);
}