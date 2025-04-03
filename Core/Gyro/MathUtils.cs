using System.Runtime.CompilerServices;

namespace HastyControls.Core.Gyro;

public static class MathUtils
{
	public const float Tau = MathF.PI * 2;
	public const float DegreesToRadians = MathF.PI / 180f;
	public const float RadiansToDegrees = 180f / MathF.PI;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Exp2(double x)
	{
		return (24 + x * (24 + x * (12 + x * (4 + x)))) * 0.041666666;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float InverseLerp(float from, float to, float value)
	{
		return (value - from) / (to - from);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Lerp(float from, float to, float t)
	{
		return from + (to - from) * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Remap(float value, float fromA, float fromB, float toA, float toB)
	{
		float t = Math.Clamp(InverseLerp(fromA, fromB, value), 0f, 1f);
		return Lerp(toA, toB, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float AngleDifference(float from, float to)
	{
		float difference = (to - from) % Tau;
		return 2.0f * difference % Tau - difference;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Wrap(float value, float min, float max)
	{
		float num = max - min;
		return min + ((value - min) % num + num) % num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float WrapAngle0ToTau(float angle)
	{
		float num = angle % Tau;
		if (num < 0f) num += Tau;
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EaseOutQuad(float t)
	{
		return t * (2 - t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EaseOutCubic(float t)
	{
		return 1 + --t * t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EaseOutQuart(float t)
	{
		t = --t * t;
		return 1 - t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EaseOutQuint(float t)
	{
		float t2 = --t * t;
		return 1 + t * t2 * t2;
	}
}
