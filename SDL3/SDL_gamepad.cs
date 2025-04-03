namespace HastyControls.SDL3;

public partial struct SDL_Gamepad
{
}

public enum SDL_GamepadType
{
	SDL_GAMEPAD_TYPE_UNKNOWN = 0,
	SDL_GAMEPAD_TYPE_STANDARD,
	SDL_GAMEPAD_TYPE_XBOX360,
	SDL_GAMEPAD_TYPE_XBOXONE,
	SDL_GAMEPAD_TYPE_PS3,
	SDL_GAMEPAD_TYPE_PS4,
	SDL_GAMEPAD_TYPE_PS5,
	SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO,
	SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_LEFT,
	SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_RIGHT,
	SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_PAIR,
	SDL_GAMEPAD_TYPE_COUNT,
}

public enum SDL_GamepadButton
{
	SDL_GAMEPAD_BUTTON_INVALID = -1,
	SDL_GAMEPAD_BUTTON_SOUTH,
	SDL_GAMEPAD_BUTTON_EAST,
	SDL_GAMEPAD_BUTTON_WEST,
	SDL_GAMEPAD_BUTTON_NORTH,
	SDL_GAMEPAD_BUTTON_BACK,
	SDL_GAMEPAD_BUTTON_GUIDE,
	SDL_GAMEPAD_BUTTON_START,
	SDL_GAMEPAD_BUTTON_LEFT_STICK,
	SDL_GAMEPAD_BUTTON_RIGHT_STICK,
	SDL_GAMEPAD_BUTTON_LEFT_SHOULDER,
	SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER,
	SDL_GAMEPAD_BUTTON_DPAD_UP,
	SDL_GAMEPAD_BUTTON_DPAD_DOWN,
	SDL_GAMEPAD_BUTTON_DPAD_LEFT,
	SDL_GAMEPAD_BUTTON_DPAD_RIGHT,
	SDL_GAMEPAD_BUTTON_MISC1,
	SDL_GAMEPAD_BUTTON_RIGHT_PADDLE1,
	SDL_GAMEPAD_BUTTON_LEFT_PADDLE1,
	SDL_GAMEPAD_BUTTON_RIGHT_PADDLE2,
	SDL_GAMEPAD_BUTTON_LEFT_PADDLE2,
	SDL_GAMEPAD_BUTTON_TOUCHPAD,
	SDL_GAMEPAD_BUTTON_MISC2,
	SDL_GAMEPAD_BUTTON_MISC3,
	SDL_GAMEPAD_BUTTON_MISC4,
	SDL_GAMEPAD_BUTTON_MISC5,
	SDL_GAMEPAD_BUTTON_MISC6,
	SDL_GAMEPAD_BUTTON_COUNT,
}

public enum SDL_GamepadAxis
{
	SDL_GAMEPAD_AXIS_INVALID = -1,
	SDL_GAMEPAD_AXIS_LEFTX,
	SDL_GAMEPAD_AXIS_LEFTY,
	SDL_GAMEPAD_AXIS_RIGHTX,
	SDL_GAMEPAD_AXIS_RIGHTY,
	SDL_GAMEPAD_AXIS_LEFT_TRIGGER,
	SDL_GAMEPAD_AXIS_RIGHT_TRIGGER,
	SDL_GAMEPAD_AXIS_COUNT,
}

public unsafe partial class SDL
{
	public delegate SDL_Gamepad* SDL_OpenGamepad(SDL_JoystickID instance_id);
	public delegate SDL_JoystickID SDL_GetGamepadID(SDL_Gamepad* gamepad);
	public delegate SDL_Joystick* SDL_GetGamepadJoystick(SDL_Gamepad* gamepad);
	public delegate byte* SDL_GetGamepadName(SDL_Gamepad* gamepad);
	public delegate SDL_GamepadType SDL_GetGamepadType(SDL_Gamepad* gamepad);
	public delegate void SDL_SetGamepadEventsEnabled(SDLBool enabled);
	public delegate SDLBool SDL_GamepadEventsEnabled();
	public delegate void SDL_UpdateGamepads();
	public delegate int SDL_GetNumGamepadTouchpads(SDL_Gamepad* gamepad);
	public delegate int SDL_GetNumGamepadTouchpadFingers(SDL_Gamepad* gamepad, int touchpad);
	public delegate SDLBool SDL_GetGamepadTouchpadFinger(SDL_Gamepad* gamepad, int touchpad, int finger, SDLBool* down, float* x, float* y, float* pressure);
	public delegate SDLBool SDL_GamepadHasSensor(SDL_Gamepad* gamepad, SDL_SensorType type);
	public delegate SDLBool SDL_SetGamepadSensorEnabled(SDL_Gamepad* gamepad, SDL_SensorType type, SDLBool enabled);
	public delegate SDLBool SDL_GamepadSensorEnabled(SDL_Gamepad* gamepad, SDL_SensorType type);
	public delegate float SDL_GetGamepadSensorDataRate(SDL_Gamepad* gamepad, SDL_SensorType type);
	public delegate SDLBool SDL_GetGamepadSensorData(SDL_Gamepad* gamepad, SDL_SensorType type, float* data, int num_values);
	public delegate SDLBool SDL_RumbleGamepad(SDL_Gamepad* gamepad, ushort low_frequency_rumble, ushort high_frequency_rumble, uint duration_ms);
	public delegate SDLBool SDL_RumbleGamepadTriggers(SDL_Gamepad* gamepad, ushort left_rumble, ushort right_rumble, uint duration_ms);
	public delegate SDLBool SDL_SetGamepadLED(SDL_Gamepad* gamepad, byte red, byte green, byte blue);
	public delegate void SDL_CloseGamepad(SDL_Gamepad* gamepad);

	public SDL_OpenGamepad OpenGamepad;
	public SDL_GetGamepadID GetGamepadID;
	public SDL_GetGamepadJoystick GetGamepadJoystick;
	public SDL_GetGamepadName GetGamepadName;
	public SDL_GetGamepadType GetGamepadType;
	public SDL_SetGamepadEventsEnabled SetGamepadEventsEnabled;
	public SDL_GamepadEventsEnabled GamepadEventsEnabled;
	public SDL_UpdateGamepads UpdateGamepads;
	public SDL_GetNumGamepadTouchpads GetNumGamepadTouchpads;
	public SDL_GetNumGamepadTouchpadFingers GetNumGamepadTouchpadFingers;
	public SDL_GetGamepadTouchpadFinger GetGamepadTouchpadFinger;
	public SDL_GamepadHasSensor GamepadHasSensor;
	public SDL_SetGamepadSensorEnabled SetGamepadSensorEnabled;
	public SDL_GamepadSensorEnabled GamepadSensorEnabled;
	public SDL_GetGamepadSensorDataRate GetGamepadSensorDataRate;
	public SDL_GetGamepadSensorData GetGamepadSensorData;
	public SDL_RumbleGamepad RumbleGamepad;
	public SDL_RumbleGamepadTriggers RumbleGamepadTriggers;
	public SDL_SetGamepadLED SetGamepadLED;
	public SDL_CloseGamepad CloseGamepad;
}
