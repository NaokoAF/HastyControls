namespace HastyControls.SDL3;

public unsafe partial class SDL
{
	public delegate int SDL_GetVersion();
	public delegate byte* SDL_GetRevision();

	public SDL_GetVersion GetVersion;
	public SDL_GetRevision GetRevision;
}
