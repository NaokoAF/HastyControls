namespace HastyControls.SDL3;

public enum SDL_JoystickID : uint;

public partial struct SDL_Joystick
{
}

public unsafe partial class SDL
{
	public delegate void SDL_UpdateJoysticks();

	public SDL_UpdateJoysticks UpdateJoysticks;
}