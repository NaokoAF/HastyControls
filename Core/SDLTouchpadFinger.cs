using System.Numerics;

namespace HastyControls.Core;

public struct SDLTouchpadFinger
{
	public bool Down;
	public Vector2 Position;
	public float Pressure;
	public ulong Timestamp;
}