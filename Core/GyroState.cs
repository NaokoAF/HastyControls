using GyroHelpers;
using GyroHelpers.GyroSpaces;
using System.Numerics;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

class GyroState
{
	public GyroInput GyroInput { get; } = new();
	public GyroProcessor GyroProcessor { get; } = new();
	public GyroOrientation DefaultOrientation { get; set; } = GyroOrientation.Normal;
	public float BiasCalibrationTime { get; set; }

	public event Action<Vector3>? BiasCalibrated;

	GyroSpace? currentGyroSpace = null;

	public Vector2 Update(bool active, float deltaTime)
	{
		if (BiasCalibrationTime > 0)
		{
			BiasCalibrationTime -= deltaTime;
			GyroInput.Calibrating = BiasCalibrationTime > 0;
			if (BiasCalibrationTime <= 0)
				BiasCalibrated?.Invoke(GyroInput.Bias);
		}

		if (!active)
		{
			GyroProcessor.Reset();
			return Vector2.Zero;
		}

		var gyroSpaceSetting = GetSetting<GyroSpaceSetting>().Value;
		var gyroOrientationSetting = GetSetting<GyroOrientationSetting>().Value;
		var gyroSmoothingThresholdSetting = GetSetting<GyroSmoothingThresholdSetting>().Value;
		var gyroSmoothingTimeSetting = GetSetting<GyroSmoothingTimeSetting>().Value;
		var gyroTighteningSetting = GetSetting<GyroTighteningSetting>().Value;

		if (currentGyroSpace != gyroSpaceSetting)
		{
			GyroProcessor.GyroSpace = CreateGyroSpace(gyroSpaceSetting);
			currentGyroSpace = gyroSpaceSetting;
		}

		if (gyroOrientationSetting == GyroOrientation.Auto)
		{
			gyroOrientationSetting = DefaultOrientation;
		}

		GyroProcessor.SmoothingThresholdDirect = gyroSmoothingThresholdSetting * MathHelper.DegreesToRadians;
		GyroProcessor.SmoothingThresholdSmooth = gyroSmoothingThresholdSetting * MathHelper.DegreesToRadians * 0.5f;
		GyroProcessor.SmoothingTime = gyroSmoothingTimeSetting;
		GyroProcessor.TighteningThreshold = gyroTighteningSetting * MathHelper.DegreesToRadians;
		GyroProcessor.Acceleration.ThresholdSlow = 0f;
		GyroProcessor.Acceleration.ThresholdFast = 0f;
		GyroProcessor.Acceleration.SensitivitySlow = 1f;
		GyroProcessor.Acceleration.SensitivityFast = 1f;

		var gyro = GyroInput.Gyro;
		gyro.Gyro = ApplyOrientation(gyro.Gyro, gyroOrientationSetting);
		gyro.Accelerometer = ApplyOrientation(gyro.Accelerometer, gyroOrientationSetting);
		gyro.Gravity = ApplyOrientation(gyro.Gravity, gyroOrientationSetting);
		
		return GyroProcessor.Update(gyro, deltaTime);
	}
	
	static IGyroSpace CreateGyroSpace(GyroSpace space) => space switch
	{
		GyroSpace.LocalYaw => new LocalGyroSpace(GyroAxis.Yaw),
		GyroSpace.LocalRoll => new LocalGyroSpace(GyroAxis.Roll),
		GyroSpace.PlayerTurn => new PlayerTurnGyroSpace(),
		GyroSpace.PlayerLean => new PlayerLeanGyroSpace(),
		_ => new LocalGyroSpace(),
	};

	static Vector3 ApplyOrientation(Vector3 vector, GyroOrientation orientation) => orientation switch
	{
		GyroOrientation.Deck => new(vector.X, -vector.Z, vector.Y),
		GyroOrientation.Ally => new(-vector.Z, -vector.X, vector.Y),
		_ => vector,
	};
}