namespace HastyControls.SDL3;

public unsafe partial class SDL
{
	public delegate byte* SDL_GetError();
	public delegate SDLBool SDL_ClearError();

	public SDL_GetError GetError;
	public SDL_ClearError ClearError;
}