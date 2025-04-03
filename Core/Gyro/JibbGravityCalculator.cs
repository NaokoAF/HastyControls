using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace HastyControls.Core.Gyro;

// adapted directly from the gyrowiki article, in combination with the implementation from GamepadMotionHelpers
// http://gyrowiki.jibbsmart.com/blog:finding-gravity-with-sensor-fusion
// https://github.com/JibbSmart/GamepadMotionHelpers/blob/main/GamepadMotion.hpp
// NOTE: none of these variables should be exposed to the user. i barely undestand them myself :/
public class JibbGravityCalculator
{
	// the time it takes in our acceleration smoothing for 'A' to get halfway to 'B'
	public float SmoothingHalfTime { get; set; } = 0.25f;

	// thresholds of trust for accelerometer shakiness. less shakiness = more trust
	public float ShakinessMinThreshold { get; set; } = 0.1f;
	public float ShakinessMaxThreshold { get; set; } = 40f;

	// when we trust the accelerometer a lot (the controller is "still"), how quickly do we correct our gravity vector?
	public float CorrectionStillRate { get; set; } = 1f;
	// when we don't trust the accelerometer (the controller is "shaky"), how quickly do we correct our gravity vector?
	public float CorrectionShakyRate { get; set; } = 0.1f;

	// if our old gravity vector is close enough to our new one, limit further corrections to this proportion of the rotation speed
	public float CorrectionGyroFactor { get; set; } = 0.1f;

	// thresholds for what's considered "close enough"
	public float CorrectionGyroMinThreshold { get; set; } = 0.05f;
	public float CorrectionGyroMaxThreshold { get; set; } = 0.25f;

	// no matter what, always apply a minimum of this much correction to our gravity vector
	public float CorrectionMinimumSpeed { get; set; } = 0.01f;

	Vector3 gravity;
	float shakiness;
	Vector3 smoothAccelerometer;

	public Vector3 Update(Vector3 gyro, Vector3 accelerometer, float deltaTime)
	{
		float gyroLength = gyro.Length();
		if (gyroLength > 0f)
		{
			// calculate gyro rotation
			Quaternion reverseRotation = Quaternion.CreateFromAxisAngle(-gyro / gyroLength, gyroLength * deltaTime);

			// rotate gravity and smoothed acceleration vectors
			gravity = Vector3.Transform(gravity, reverseRotation);
			smoothAccelerometer = Vector3.Transform(smoothAccelerometer, reverseRotation);
		}

		float accelMagnitude = accelerometer.Length();
		if (accelMagnitude > 0f)
		{
			// calculate update rate independant interpolation factor
			float smoothFactor = (float)MathUtils.Exp2(-deltaTime / SmoothingHalfTime);
			smoothAccelerometer = Vector3.Lerp(accelerometer, smoothAccelerometer, smoothFactor); // rolling average

			// shakiness is the difference between the accelerometer and the smoothed accelerometer values
			// we also always lower it over time
			shakiness *= smoothFactor;
			shakiness = MathF.Max(shakiness, (accelerometer - smoothAccelerometer).Length());

			// calculate correction rate
			Vector3 accelerometerDir = accelerometer / accelMagnitude;
			Vector3 gravToAccel = -accelerometerDir - gravity;
			float gravToAccelLength = gravToAccel.Length();
			Vector3 gravToAccelDir = gravToAccel / gravToAccelLength;
			float correctionRate = CalculateCorrectionRate(gyroLength, gravity, gravToAccelLength);

			// apply gravity correction
			Vector3 gravToAccelDelta = gravToAccelDir * correctionRate * deltaTime;
			if (gravToAccelDelta.LengthSquared() < gravToAccelLength * gravToAccelLength)
			{
				gravity += gravToAccelDelta;
			}
			else
			{
				gravity = -accelerometerDir;
			}
		}

		return gravity;
	}

	public void Reset()
	{
		shakiness = 0f;
		gravity = Vector3.Zero;
		smoothAccelerometer = Vector3.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	float CalculateCorrectionRate(float gyroSpeed, Vector3 gravity, float gravToAccelLength)
	{
		Debug.Assert(ShakinessMaxThreshold > ShakinessMinThreshold);
		Debug.Assert(CorrectionGyroMaxThreshold > CorrectionGyroMinThreshold);

		float correctionRate = MathUtils.Remap(shakiness, ShakinessMinThreshold, ShakinessMaxThreshold, CorrectionStillRate, CorrectionShakyRate);

		// limit correction rate in proportion to rotation rate
		float correctionLimit = MathF.Max(gyroSpeed * gravity.Length() * CorrectionGyroFactor, CorrectionMinimumSpeed);
		if (correctionRate > correctionLimit)
		{
			correctionRate = MathUtils.Remap(gravToAccelLength, CorrectionGyroMinThreshold, CorrectionGyroMaxThreshold, correctionLimit, correctionRate);
		}
		return correctionRate;
	}
}
