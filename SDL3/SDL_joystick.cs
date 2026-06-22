namespace HastyControls.SDL3;

public enum SDL_JoystickID : uint;

public partial struct SDL_Joystick
{
}

public unsafe partial class SDL
{
	public delegate void SDL_UpdateJoysticks();

	public delegate ushort SDL_GetJoystickVendorForID(SDL_JoystickID instance_id);

	public delegate ushort SDL_GetJoystickProductForID(SDL_JoystickID instance_id);

	public SDL_UpdateJoysticks UpdateJoysticks;
	public SDL_GetJoystickVendorForID GetJoystickVendorForID;
	public SDL_GetJoystickProductForID GetJoystickProductForID;
}