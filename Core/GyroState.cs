using GyroHelpers;
using GyroHelpers.GyroSpaces;
using System.Numerics;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

class GyroState
{
	public GyroInput GyroInput { get; } = new();
	public GyroProcessor GyroProcessor { get; } = new();
	public float BiasCalibrationTime { get; set; }

	public event Action<Vector3>? BiasCalibrated;

	GyroSpace currentGyroSpace = GyroSpace.LocalYaw;

	public Vector2 Update(float deltaTime)
	{
		if (BiasCalibrationTime > 0)
		{
			BiasCalibrationTime -= deltaTime;
			GyroInput.Calibrating = BiasCalibrationTime > 0;
			if (BiasCalibrationTime <= 0)
				BiasCalibrated?.Invoke(GyroInput.Bias);
		}

		var gyroSpaceSetting = GetSetting<GyroSpaceSetting>().Value;
		var gyroSmoothingThresholdSetting = GetSetting<GyroSmoothingThresholdSetting>().Value;
		var gyroSmoothingTimeSetting = GetSetting<GyroSmoothingTimeSetting>().Value;
		var gyroTighteningSetting = GetSetting<GyroTighteningSetting>().Value;

		if (currentGyroSpace != gyroSpaceSetting)
		{
			GyroProcessor.GyroSpace = CreateGyroSpace(gyroSpaceSetting);
			currentGyroSpace = gyroSpaceSetting;
		}

		GyroProcessor.SmoothingThresholdDirect = gyroSmoothingThresholdSetting * MathHelper.DegreesToRadians;
		GyroProcessor.SmoothingThresholdSmooth = gyroSmoothingThresholdSetting * MathHelper.DegreesToRadians * 0.5f;
		GyroProcessor.SmoothingTime = gyroSmoothingTimeSetting;
		GyroProcessor.TighteningThreshold = gyroTighteningSetting * MathHelper.DegreesToRadians;
		GyroProcessor.Acceleration.ThresholdSlow = 0f;
		GyroProcessor.Acceleration.ThresholdFast = 0f;
		GyroProcessor.Acceleration.SensitivitySlow = 1f;
		GyroProcessor.Acceleration.SensitivityFast = 1f;

		return GyroProcessor.Update(GyroInput.Gyro, deltaTime);
	}

	static IGyroSpace CreateGyroSpace(GyroSpace space) => space switch
	{
		GyroSpace.LocalYaw => new LocalGyroSpace(GyroAxis.Yaw),
		GyroSpace.LocalRoll => new LocalGyroSpace(GyroAxis.Roll),
		GyroSpace.PlayerTurn => new PlayerTurnGyroSpace(),
		GyroSpace.PlayerLean => new PlayerLeanGyroSpace(),
		_ => new LocalGyroSpace(),
	};

	public void Reset()
	{
		GyroProcessor.Reset();
	}
}