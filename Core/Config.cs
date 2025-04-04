namespace HastyControls.Core;

public class Config
{
	public bool GeneralDisableAbilitiesInSafeZones { get; set; }

	public bool GyroEnabled { get; set; }
	public GyroSpace GyroSpace { get; set; }
	public ControllerButton GyroButton { get; set; }
	public GyroButtonMode GyroButtonMode { get; set; }
	public ControllerButton GyroCalibrateButton { get; set; }
	public float GyroSensitivity { get; set; }
	public float GyroSensitivityRatio { get; set; }
	public float GyroTightening { get; set; }
	public float GyroSmoothingThreshold { get; set; }
	public float GyroSmoothingTime { get; set; }
	public bool GyroDisableWhenWalking { get; set; }

	public float SticksDeadzones { get; set; }
	public float SticksPowerCurve { get; set; }
	public float SticksDegreesPerSecond { get; set; }
	public float SticksSensitivityRatio { get; set; }

	public bool AutoLookHorEnabled { get; set; }
	public bool AutoLookVerEnabled { get; set; }
	public float AutoLookHorSpeed { get; set; }
	public float AutoLookHorStrength { get; set; }
	public float AutoLookVerSpeed { get; set; }
	public float AutoLookVerStrength { get; set; }
	public float AutoLookVerBaseAngle { get; set; }

	public float RumbleIntensity { get; set; }
	public float RumbleOnDamage { get; set; }
	public float RumbleOnLand { get; set; }
	public float RumbleOnFastRun { get; set; }
	public float RumbleOnSparkPickup { get; set; }
	public float RumbleOnBoostRing { get; set; }
	public float RumbleOnBoardBoost { get; set; }
	public float RumbleOnGrapple { get; set; }
	public float RumbleOnFly { get; set; }
}
