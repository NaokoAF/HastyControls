using GyroHelpers;
using GyroHelpers.GyroSpaces;
using System.Numerics;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

internal class GyroState
{
	public GyroOrientation DefaultOrientation { get; set; } = GyroOrientation.Normal;
	public float BiasCalibrationTime { get; set; }

	public event Action<Vector3>? BiasCalibrated;

	private readonly GyroInput gyroInput = new();
	private readonly GyroProcessor gyroProcessor = new();
	private GyroSpace? currentGyroSpace;
	private GyroOrientation currentOrientation = GyroOrientation.Normal;

	public GyroState()
	{
		gyroProcessor.Acceleration.ThresholdSlow = 0f;
		gyroProcessor.Acceleration.ThresholdFast = 0f;
		gyroProcessor.Acceleration.SensitivitySlow = 1f;
		gyroProcessor.Acceleration.SensitivityFast = 1f;
	}
	
	public void Begin()
	{
		gyroInput.Begin();
		
		var gyroSpaceSetting = GetSetting<GyroSpaceSetting>().Value;
		if (currentGyroSpace != gyroSpaceSetting)
		{
			currentGyroSpace = gyroSpaceSetting;
			gyroProcessor.GyroSpace = currentGyroSpace switch
			{
				GyroSpace.LocalYaw => new LocalGyroSpace(GyroAxis.Yaw),
				GyroSpace.LocalRoll => new LocalGyroSpace(GyroAxis.Roll),
				GyroSpace.PlayerTurn => new PlayerTurnGyroSpace(),
				GyroSpace.PlayerLean => new PlayerLeanGyroSpace(),
				_ => new LocalGyroSpace(GyroAxis.Yaw),
			};
		}
		
		var gyroOrientationSetting = GetSetting<GyroOrientationSetting>().Value;
		currentOrientation = gyroOrientationSetting == GyroOrientation.Auto ? DefaultOrientation : gyroOrientationSetting;
		
		var gyroSmoothingThresholdSetting = GetSetting<GyroSmoothingThresholdSetting>().Value;
		var gyroSmoothingTimeSetting = GetSetting<GyroSmoothingTimeSetting>().Value;
		var gyroTighteningSetting = GetSetting<GyroTighteningSetting>().Value;
		gyroProcessor.SmoothingThresholdDirect = gyroSmoothingThresholdSetting * MathHelper.DegreesToRadians;
		gyroProcessor.SmoothingThresholdSmooth = gyroSmoothingThresholdSetting * MathHelper.DegreesToRadians * 0.5f;
		gyroProcessor.SmoothingTime = gyroSmoothingTimeSetting;
		gyroProcessor.TighteningThreshold = gyroTighteningSetting * MathHelper.DegreesToRadians;
	}
	
	public Vector2 Update(bool active, float deltaTime)
	{
		if (BiasCalibrationTime > 0)
		{
			BiasCalibrationTime -= deltaTime;
			gyroInput.Calibrating = BiasCalibrationTime > 0;
			if (BiasCalibrationTime <= 0)
				BiasCalibrated?.Invoke(gyroInput.Bias);
		}

		if (!active)
		{
			gyroProcessor.Reset();
			return Vector2.Zero;
		}

		return gyroProcessor.Update(gyroInput.Gyro, deltaTime);
	}
	
	public void AddGyroSample(Vector3 gyro, ulong timestamp)
	{
		gyro = currentOrientation switch
		{
			GyroOrientation.Deck => new(gyro.X, -gyro.Z, gyro.Y),
			GyroOrientation.Ally => new(-gyro.Z, -gyro.X, gyro.Y),
			_ => gyro,
		};
		
		gyroInput.AddGyroSample(gyro, timestamp);
	}

	public void AddAccelerometerSample(Vector3 accel, ulong timestamp)
	{
		accel = currentOrientation switch
		{
			GyroOrientation.Deck => new(accel.X, -accel.Z, accel.Y),
			GyroOrientation.Ally => new(-accel.Z, -accel.X, accel.Y),
			_ => accel,
		};
		
		gyroInput.AddAccelerometerSample(accel, timestamp);
	}
}