using System.Numerics;

namespace HastyControls.Core.Gyro;

public class FlickStick
{
	public float FlickThreshold { get; set; } = 0.9f;
	public float FlickTime { get; set; } = 0.1f;
	public float SmoothTime { get => smoothing.SmoothTime; set => smoothing.SmoothTime = value; }
	public float SmoothThreshold
	{
		get => smoothing.ThresholdDirect * MathUtils.RadiansToDegrees;
		set
		{
			smoothing.ThresholdDirect = value * MathUtils.DegreesToRadians;
			smoothing.ThresholdSmooth = value * MathUtils.DegreesToRadians * 0.5f;
		}
	}

	public bool FlickStickActive => flickStickActive;

	bool flickStickActive;
	float flickProgress = 1f;
	float flickAngle;
	float flickAccumulatedAngle;
	float flickImmediateAngle;
	TieredSmoothing1D smoothing = new(0.1f, 0.5f * MathUtils.DegreesToRadians, 1f * MathUtils.DegreesToRadians, 256);

	Vector2 lastStick;

	// called for every received input sample
	public void Input(Vector2 stick)
	{
		float lastMagnitudeSqr = lastStick.LengthSquared();
		float magnitudeSqr = stick.LengthSquared();
		float thresholdSqr = FlickThreshold * FlickThreshold;
		if (magnitudeSqr >= thresholdSqr)
		{
			float stickAngle = MathF.Atan2(-stick.X, -stick.Y);
			if (lastMagnitudeSqr < thresholdSqr)
			{
				// stick just crossed the threshold. initiate flick at this angle
				if (FlickTime > 0)
				{
					flickProgress = 0;
					flickAngle = stickAngle;
				}
				else
				{
					// no flick animation. increment a value that gets added to GetDelta
					flickImmediateAngle += stickAngle;
				}
			}
			else
			{
				// we're still outside the threshold. calculate the angle change
				float lastStickAngle = MathF.Atan2(-lastStick.X, -lastStick.Y);
				flickAccumulatedAngle += stickAngle - lastStickAngle;
				flickAccumulatedAngle = MathUtils.Wrap(flickAccumulatedAngle, -MathF.PI, MathF.PI);
			}
			flickStickActive = true;
		}
		else
		{
			// turn cleanup
			if (lastMagnitudeSqr >= thresholdSqr)
			{
				smoothing.Reset();
			}
			flickStickActive = false;
		}
		lastStick = stick;
	}

	// called once per frame to rotate camera
	public float Update(float deltaTime)
	{
		float result = flickImmediateAngle;
		flickImmediateAngle = 0f;

		// update turn
		if (flickAccumulatedAngle != 0f)
		{
			result += smoothing.Apply(flickAccumulatedAngle, deltaTime);
			flickAccumulatedAngle = 0f;
		}

		// update flick animation
		if (flickProgress < 1f && FlickTime > 0)
		{
			// move progress forward
			float lastFlickProgress = flickProgress;
			flickProgress = MathF.Min(flickProgress + (float)(deltaTime / FlickTime), 1f);

			// apply easing (optional but looks weird without it)
			float lastT = MathUtils.EaseOutCubic(lastFlickProgress);
			float t = MathUtils.EaseOutCubic(flickProgress);
			result += (t - lastT) * flickAngle;
		}

		return result;
	}

	public void Flush()
	{
		flickProgress = 1f;
		flickAccumulatedAngle = 0f;
		flickImmediateAngle = 0f;
		smoothing.Reset();
	}
}
