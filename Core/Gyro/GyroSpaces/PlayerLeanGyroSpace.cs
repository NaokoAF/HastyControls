using System.Numerics;

namespace HastyControls.Core.Gyro.GyroSpaces;

public class PlayerLeanGyroSpace : IGyroSpace
{
	public float RollRelaxFactor { get; set; } = 1.15f;
	public bool RequiresGravity => true;

	public Vector2 Transform(Vector3 gyro, Vector3 gravity)
	{
		// project pitch axis onto gravity plane
		Vector3 pitchVector = Vector3.UnitX - gravity * gravity.X;

		// normalize. it'll be zero if pitch and gravity are parallel, which we ignore
		if (pitchVector != Vector3.Zero)
		{
			Vector3 rollVector = Vector3.Cross(pitchVector, gravity);
			if (rollVector != Vector3.Zero)
			{
				rollVector = Vector3.Normalize(rollVector);

				// some info about the controller's orientation that we'll use to smooth over boundaries
				float flatness = MathF.Abs(gravity.Y); // 1 when controller is flat
				float upness = MathF.Abs(gravity.Z); // 1 when controller is upright
				float sideReduction = Math.Clamp((MathF.Max(flatness, upness) - 0.125f) / 0.125f, 0f, 1f);

				float worldRoll = gyro.Y * rollVector.Y + gyro.Z * rollVector.Z; // dot product but just yaw and roll
				float gyroMagnitude = MathF.Sqrt(gyro.Y * gyro.Y + gyro.Z * gyro.Z); // magnitude but just yaw and roll
				float yaw = -Math.Sign(worldRoll) * sideReduction * MathF.Min(MathF.Abs(worldRoll) * RollRelaxFactor, gyroMagnitude);
				return new Vector2(gyro.X, yaw);
			}
		}

		return new Vector2(gyro.X, 0f);
	}
}
