using HastyControls.Core.Gyro;
using HastyControls.Core.Gyro.GyroSpaces;
using System.Numerics;

namespace HastyControls.Core;

class GyroState
{
	public GyroAiming Gyro { get; } = new();
	public float BiasCalibrationTime { get; set; }

	public event Action<Vector3>? BiasCalibrated;

	JibbGravityCalculator gravityCalculator = new();
	ulong? prevTimestamp;

	public void UpdateConfig(Config config)
	{
		IGyroSpace gyroSpace;
		switch (config.GyroSpace)
		{
			case GyroSpace.LocalYaw: gyroSpace = new LocalYawGyroSpace(); break;
			case GyroSpace.LocalRoll: gyroSpace = new LocalRollGyroSpace(); break;
			case GyroSpace.PlayerTurn: gyroSpace = new PlayerTurnGyroSpace(); break;
			case GyroSpace.PlayerLean: gyroSpace = new PlayerLeanGyroSpace(); break;
			default: gyroSpace = new LocalYawGyroSpace(); break;
		}

		Gyro.GyroSpace = gyroSpace;
		Gyro.SmoothingThresholdDirect = config.GyroSmoothingThreshold;
		Gyro.SmoothingThresholdSmooth = config.GyroSmoothingThreshold * 0.5f;
		Gyro.SmoothingTime = config.GyroSmoothingTime;
		Gyro.TighteningThreshold = config.GyroTightening;
		Gyro.Acceleration.ThresholdSlow = 0f;
		Gyro.Acceleration.ThresholdFast = 0f;
		Gyro.Acceleration.SensitivitySlow = 1f;
		Gyro.Acceleration.SensitivityFast = 1f;

		prevTimestamp = null;
	}

	public void Input(Vector3 gyro, Vector3 accel, ulong timestamp)
	{
		float deltaTime = (timestamp - (prevTimestamp ?? timestamp)) / 1000000000f;
		prevTimestamp = timestamp;

		if (BiasCalibrationTime > 0)
		{
			BiasCalibrationTime -= deltaTime;
			Gyro.CalibratingBias = BiasCalibrationTime > 0;
			if (BiasCalibrationTime <= 0)
				BiasCalibrated?.Invoke(Gyro.Bias);
		}

		Vector3 gravity = Gyro.GyroSpace.RequiresGravity ? gravityCalculator.Update(gyro, accel, deltaTime) : Vector3.Zero;
		Gyro.Input(gyro, accel, gravity, deltaTime);
	}

	public Vector2 Update(float deltaTime)
	{
		return Gyro.Update(deltaTime);
	}

	public void Flush()
	{
		Gyro.Flush();
	}
}