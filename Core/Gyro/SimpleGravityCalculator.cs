using System.Numerics;

namespace HastyControls.Core.Gyro;

public class SimpleGravityCalculator
{
	public float NudgeFactor { get; set; } = 0.02f;

	Vector3 gravity;

	public Vector3 Update(Vector3 gyro, Vector3 accelerometer, float deltaTime)
	{
		float gyroLength = gyro.Length();
		if (gyroLength > 0f)
		{
			// calculate gyro rotation
			Quaternion reverseRotation = Quaternion.CreateFromAxisAngle(-gyro / gyroLength, gyroLength * deltaTime);

			// rotate gravity vector
			gravity = Vector3.Transform(gravity, reverseRotation);
		}

		float accelMagnitude = accelerometer.Length();
		if (accelMagnitude > 0f)
		{
			// nudge towards gravity according to current acceleration
			Vector3 accelerometerDir = accelerometer / accelMagnitude;
			gravity += (-accelerometerDir - gravity) * NudgeFactor;
		}
		return gravity;
	}

	public void Reset()
	{
		gravity = Vector3.Zero;
	}
}
