namespace HastyControls.SDL3;

public readonly record struct SDLBool
{
	private readonly byte value;

	internal const byte FALSE_VALUE = 0;
	internal const byte TRUE_VALUE = 1;

	[Obsolete("Never explicitly construct an SDL bool.")]
	public SDLBool()
	{
	}

	internal SDLBool(byte value)
	{
		this.value = value;
	}

	public static implicit operator bool(SDLBool b) => b.value != FALSE_VALUE;

	public static implicit operator SDLBool(bool b) => new SDLBool(b ? TRUE_VALUE : FALSE_VALUE);

	public bool Equals(SDLBool other) => (bool)other == (bool)this;

	public override int GetHashCode() => ((bool)this).GetHashCode();
}

public unsafe partial class SDL
{
	public delegate IntPtr SDL_malloc(nuint size);
	public delegate IntPtr SDL_calloc(nuint nmemb, nuint size);
	public delegate IntPtr SDL_realloc(IntPtr mem, nuint size);
	public delegate void SDL_free(IntPtr mem);

	public SDL_malloc malloc;
	public SDL_calloc calloc;
	public SDL_realloc realloc;
	public SDL_free free;
}
