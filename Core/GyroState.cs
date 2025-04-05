using HastyControls.Core.Gyro;
using HastyControls.Core.Gyro.GyroSpaces;
using System.Numerics;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

class GyroState
{
	public GyroAiming Gyro { get; } = new();
	public float BiasCalibrationTime { get; set; }

	public event Action<Vector3>? BiasCalibrated;

	JibbGravityCalculator gravityCalculator = new();
	ulong? prevTimestamp;
	GyroSpace currentGyroSpace = GyroSpace.LocalYaw;

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
		var gyroSpaceSetting = GetSetting<GyroSpaceSetting>().Value;
		var gyroSmoothingThresholdSetting = GetSetting<GyroSmoothingThresholdSetting>().Value;
		var gyroSmoothingTimeSetting = GetSetting<GyroSmoothingTimeSetting>().Value;
		var gyroTighteningSetting = GetSetting<GyroTighteningSetting>().Value;

		if (currentGyroSpace != gyroSpaceSetting)
		{
			Gyro.GyroSpace = CreateGyroSpace(gyroSpaceSetting);
			currentGyroSpace = gyroSpaceSetting;
		}

		Gyro.SmoothingThresholdDirect = gyroSmoothingThresholdSetting;
		Gyro.SmoothingThresholdSmooth = gyroSmoothingThresholdSetting * 0.5f;
		Gyro.SmoothingTime = gyroSmoothingTimeSetting;
		Gyro.TighteningThreshold = gyroTighteningSetting;
		Gyro.Acceleration.ThresholdSlow = 0f;
		Gyro.Acceleration.ThresholdFast = 0f;
		Gyro.Acceleration.SensitivitySlow = 1f;
		Gyro.Acceleration.SensitivityFast = 1f;

		return Gyro.Update(deltaTime);
	}

	static IGyroSpace CreateGyroSpace(GyroSpace space) => space switch
	{
		GyroSpace.LocalYaw => new LocalYawGyroSpace(),
		GyroSpace.LocalRoll => new LocalRollGyroSpace(),
		GyroSpace.PlayerTurn => new PlayerTurnGyroSpace(),
		GyroSpace.PlayerLean => new PlayerLeanGyroSpace(),
		_ => new LocalYawGyroSpace(),
	};

	public void Flush()
	{
		Gyro.Flush();
	}
}