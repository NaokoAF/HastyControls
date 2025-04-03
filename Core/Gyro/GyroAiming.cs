using HastyControls.Core.Gyro.GyroSpaces;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace HastyControls.Core.Gyro;

public class GyroAiming
{
	public IGyroSpace GyroSpace { get; set; } = new LocalYawGyroSpace();
	public GyroAcceleration Acceleration { get; } = new GyroAcceleration();
	public float TighteningThreshold { get; set; } = 5f;
	public float SmoothingTime { get => smoothing.SmoothTime; set => smoothing.SmoothTime = value; }
	public float SmoothingThresholdDirect { get => smoothing.ThresholdDirect * MathUtils.RadiansToDegrees; set => smoothing.ThresholdDirect = value * MathUtils.DegreesToRadians; }
	public float SmoothingThresholdSmooth { get => smoothing.ThresholdSmooth * MathUtils.RadiansToDegrees; set => smoothing.ThresholdSmooth = value * MathUtils.DegreesToRadians; }
	public bool CalibratingBias { get; set; }

	public Vector3 Bias => gyroBias;

	TieredSmoothing2D smoothing = new(0.1f, 0.5f * MathUtils.DegreesToRadians, 1f * MathUtils.DegreesToRadians, 256);

	Vector3 gyroBias;
	int gyroBiasSampleCount;
	Vector3 gyroAverage;
	int gyroAverageSampleCount;
	Vector3 lastGyroAverage;
	Vector3 gravity;

	// called for every received input sample
	public void Input(Vector3 gyro, Vector3 accelerometer, Vector3 gravity, float deltaTime)
	{
		// calibrate
		if (CalibratingBias)
		{
			gyroBiasSampleCount++;
			gyroBias += (gyro - gyroBias) / gyroBiasSampleCount;
			Flush();
			return;
		}

		// unbias
		gyroBiasSampleCount = 0;
		gyro -= gyroBias;

		// add to average
		gyroAverageSampleCount++;
		gyroAverage += (gyro - gyroAverage) / gyroAverageSampleCount;
		lastGyroAverage = gyroAverage;
		this.gravity = gravity;
	}

	// called once per frame to rotate camera
	public Vector2 Update(float deltaTime)
	{
		// use previous gyro value if no input samples were received this frame
		// this makes it so the gyro is "held" in a direction on frames between received samples
		// this is especially useful for switch pro controllers which only poll at 60 hz, and for high refresh rate monitors above 250 hz
		Vector3 gyro = gyroAverageSampleCount > 0 ? gyroAverage : lastGyroAverage;

		// post processing
		Vector3 gravityNormalized = gravity != Vector3.Zero ? Vector3.Normalize(gravity) : Vector3.Zero;
		Vector2 result = GyroSpace.Transform(gyro, gravityNormalized);
		result = GetTightenedInput(result, TighteningThreshold * MathUtils.DegreesToRadians);
		result = Acceleration.Transform(result, deltaTime);
		result = smoothing.Apply(result, deltaTime);
		result *= deltaTime;

		// flush
		gyroAverage = Vector3.Zero;
		gyroAverageSampleCount = 0;
		return result;
	}

	public void Flush()
	{
		gyroAverage = Vector3.Zero;
		gyroAverageSampleCount = 0;
		lastGyroAverage = Vector3.Zero;
		smoothing.Reset();
	}

	// squeezes everything below threshold down to 0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Vector2 GetTightenedInput(Vector2 input, float threshold)
	{
		float magnitude = input.Length();
		if (magnitude < threshold)
		{
			return input * (magnitude / threshold);
		}
		else
		{
			return input;
		}
	}
}
