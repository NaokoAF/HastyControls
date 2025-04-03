using System.Numerics;

namespace HastyControls.Core.Gyro.GyroSpaces;

public class PlayerTurnGyroSpace : IGyroSpace
{
	public float YawRelaxFactor { get; set; } = 2f;
	public bool RequiresGravity => true;

	public Vector2 Transform(Vector3 gyro, Vector3 gravity)
	{
		// use world yaw for yaw direction, local combined yaw for magnitude
		float worldYaw = gyro.Y * gravity.Y + gyro.Z * gravity.Z; // dot product but just yaw and roll
		float gyroMagnitude = MathF.Sqrt(gyro.Y * gyro.Y + gyro.Z * gyro.Z); // magnitude but just yaw and roll

		//float yaw = -Math.Sign(worldYaw) * gyroMagnitude;
		float yaw = -Math.Sign(worldYaw) * MathF.Min(MathF.Abs(worldYaw) * YawRelaxFactor, gyroMagnitude);

		return new Vector2(gyro.X, yaw);
	}
}
