using System.Numerics;

namespace HastyControls.Core.Gyro.GyroSpaces;

public interface IGyroSpace
{
	bool RequiresGravity { get; }
	Vector2 Transform(Vector3 gyro, Vector3 gravity);
}
