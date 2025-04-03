using System.Numerics;

namespace HastyControls.Core.Gyro.GyroSpaces;

public class LocalRollGyroSpace : IGyroSpace
{
	public bool RequiresGravity => false;

	public Vector2 Transform(Vector3 gyro, Vector3 gravity)
	{
		return new Vector2(gyro.X, -gyro.Z);
	}
}
