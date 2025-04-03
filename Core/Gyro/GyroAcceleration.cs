using System.Numerics;

namespace HastyControls.Core.Gyro;

public class GyroAcceleration
{
	public float ThresholdSlow { get; set; } = 0f;
	public float ThresholdFast { get; set; } = 75f;
	public float SensitivitySlow { get; set; } = 1f;
	public float SensitivityFast { get; set; } = 2f;

	public Vector2 Transform(Vector2 gyro, float deltaTime)
	{
		if (ThresholdFast <= ThresholdSlow)
			return gyro * SensitivitySlow;

		// map gyro speed from (ThresholdSlow -> ThresholdFast) to (SensitivitySlow -> SensitivityFast)
		float sensitivty = MathUtils.Remap(gyro.Length() * deltaTime, ThresholdSlow * MathUtils.DegreesToRadians, ThresholdFast * MathUtils.DegreesToRadians, SensitivitySlow, SensitivityFast);
		return gyro * sensitivty;
	}
}
