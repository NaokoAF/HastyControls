namespace HastyControls.SDL3;

public readonly record struct SDLBool
{
	private readonly byte value;

	internal SDLBool(byte value)
	{
		this.value = value;
	}

	public static implicit operator bool(SDLBool b) => b.value != 0;

	public static implicit operator SDLBool(bool b) => new(b ? (byte)1 : (byte)0);

	public bool Equals(SDLBool other) => (bool)other == (bool)this;

	public override int GetHashCode() => ((bool)this).GetHashCode();
}

public unsafe partial class SDL
{
	public delegate nint SDL_malloc(nuint size);

	public delegate nint SDL_calloc(nuint nmemb, nuint size);

	public delegate nint SDL_realloc(nint mem, nuint size);

	public delegate void SDL_free(nint mem);

	public SDL_malloc malloc;
	public SDL_calloc calloc;
	public SDL_realloc realloc;
	public SDL_free free;
}