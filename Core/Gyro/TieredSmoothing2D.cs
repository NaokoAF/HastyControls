using System.Numerics;

namespace HastyControls.Core.Gyro;

public class TieredSmoothing2D
{
	public float ThresholdSmooth { get; set; }
	public float ThresholdDirect { get; set; }
	public float SmoothTime
	{
		get => movingAverageX.TimeWindow;
		set
		{
			movingAverageX.TimeWindow = value;
			movingAverageY.TimeWindow = value;
		}
	}

	TimedMovingAverage movingAverageX;
	TimedMovingAverage movingAverageY;

	public TieredSmoothing2D(float smoothTime, float thresholdSmooth, float thresholdDirect, int inputsPerSecond)
	{
		ThresholdSmooth = thresholdSmooth;
		ThresholdDirect = thresholdDirect;
		movingAverageX = new(smoothTime, inputsPerSecond);
		movingAverageY = new(smoothTime, inputsPerSecond);
	}

	public Vector2 Apply(Vector2 input, float deltaTime)
	{
		if (SmoothTime <= 0 || ThresholdDirect - ThresholdSmooth <= 0)
			return input;

		// maps input magnitude to a value between 0 and 1, where 0 is thresholdSmooth and 1 is thresholdDirect
		// we then interpolate between direct and smoothed inputs based on this value
		// this means inputs below thresholdSmooth get smoothed, inputs above thresholdDirect dont, and everything in between gets a mix of both
		float inputMagnitude = input.Length();
		float weight = Math.Clamp(MathUtils.InverseLerp(ThresholdSmooth, ThresholdDirect, inputMagnitude), 0f, 1f);
		Vector2 directInput = input * weight;
		Vector2 smootherInput = new Vector2(
			movingAverageX.Add(deltaTime, input.X * (1f - weight)),
			movingAverageY.Add(deltaTime, input.Y * (1f - weight))
		);
		return directInput + smootherInput;
	}

	public void Reset()
	{
		movingAverageX.Reset();
		movingAverageY.Reset();
	}
}