namespace HastyControls.Core;

public record struct SDLVersion
{
	public int Major => (value) / 1000000;
	public int Minor => ((value) / 1000) % 1000;
	public int Micro => (value) % 1000;

	private readonly int value;

	public SDLVersion(int value)
	{
		this.value = value;
	}
}
