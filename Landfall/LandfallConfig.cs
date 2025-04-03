using HastyControls.Configuration;
using HastyControls.Core;
using IniParser;
using IniParser.Configuration;

namespace HastyControls.Landfall;

public class LandfallConfig
{
	public readonly IniConfigEntryBoolean GeneralDisableAbilitiesInSafeZones;

	public readonly IniConfigEntryBoolean GyroEnabled;
	public readonly IniConfigEntryEnum<GyroSpace> GyroSpace;
	public readonly IniConfigEntryEnum<ControllerButton> GyroButton;
	public readonly IniConfigEntryEnum<GyroButtonMode> GyroButtonMode;
	public readonly IniConfigEntryEnum<ControllerButton> GyroCalibrateButton;
	public readonly IniConfigEntryNumber GyroSensitivity;
	public readonly IniConfigEntryNumber GyroSensitivityRatio;
	public readonly IniConfigEntryNumber GyroTightening;
	public readonly IniConfigEntryNumber GyroSmoothingThreshold;
	public readonly IniConfigEntryNumber GyroSmoothingTime;
	public readonly IniConfigEntryBoolean GyroDisableWhenWalking;
	public readonly IniConfigEntryNumber GyroLevelStartPauseLength;

	public readonly IniConfigEntryNumber SticksDeadzones;
	public readonly IniConfigEntryNumber SticksPowerCurve;
	public readonly IniConfigEntryNumber SticksDegreesPerSecond;
	public readonly IniConfigEntryNumber SticksSensitivityRatio;

	public readonly IniConfigEntryBoolean AutoLookHorEnabled;
	public readonly IniConfigEntryBoolean AutoLookVerEnabled;
	public readonly IniConfigEntryNumber AutoLookHorSpeed;
	public readonly IniConfigEntryNumber AutoLookHorStrength;
	public readonly IniConfigEntryNumber AutoLookVerSpeed;
	public readonly IniConfigEntryNumber AutoLookVerStrength;
	public readonly IniConfigEntryNumber AutoLookVerBaseAngle;

	public readonly IniConfigEntryNumber RumbleIntensity;
	public readonly IniConfigEntryNumber RumbleOnDamage;
	public readonly IniConfigEntryNumber RumbleOnLand;
	public readonly IniConfigEntryNumber RumbleOnFastRun;
	public readonly IniConfigEntryNumber RumbleOnSparkPickup;
	public readonly IniConfigEntryNumber RumbleOnBoostRing;
	public readonly IniConfigEntryNumber RumbleOnBoardBoost;
	public readonly IniConfigEntryNumber RumbleOnGrapple;
	public readonly IniConfigEntryNumber RumbleOnFly;

	IniConfigFile file;
	IniConfigCategory generalCategory;
	IniConfigCategory gyroCategory;
	IniConfigCategory sticksCategory;
	IniConfigCategory autoLookCategory;
	IniConfigCategory rumbleCategory;

	static IniDataParser iniParser = new();
	static IniDataFormatter iniFormatter = new();
	static IniFormattingConfiguration iniFormattingConfiguration = new()
	{
		NewLineAfterProperty = true,
	};

	static LandfallConfig()
	{
		iniParser.Configuration.SkipInvalidLines = true;
		iniParser.Configuration.AllowDuplicateSections = true;
		iniParser.Configuration.DuplicatePropertiesBehaviour = IniParserConfiguration.EDuplicatePropertiesBehaviour.AllowAndKeepLastValue;
	}

	public LandfallConfig(IniConfigFile file)
	{
		this.file = file;
		generalCategory = file.CreateCategory("General");
		gyroCategory = file.CreateCategory("Gyro");
		sticksCategory = file.CreateCategory("Sticks");
		autoLookCategory = file.CreateCategory("AutoLook");
		rumbleCategory = file.CreateCategory("Rumble");

		GeneralDisableAbilitiesInSafeZones = generalCategory.CreateEntry("DisableAbilitiesInSafeZones", true,
			"Wether to disable abilities in the shop and healing areas."
		);

		GyroEnabled = gyroCategory.CreateEntry("GyroEnabled", true,
			"Enable motion controls on supported controllers (DualShock 4 and DualSense)."
		);
		GyroSpace = gyroCategory.CreateEntryEnum("GyroSpace", Core.GyroSpace.PlayerTurn,
			"Algorithm used to convert real world movements to the in game camera.\n" +
			"'Local' recommended for handhelds (Steam Deck), 'Player' recommended for everything else.\n" +
			"'Turn' if you prefer to turn your controller left and right to look around, 'Lean' if you prefer to tilt the controller instead."
		);
		GyroButton = gyroCategory.CreateEntryEnum("GyroButton", ControllerButton.RightStick,
			"Controller button to enable/disable gyro."
		);
		GyroButtonMode = gyroCategory.CreateEntryEnum("GyroButtonMode", Core.GyroButtonMode.Off,
			"Behavior of gyro button. Off disables motion controls while the button is held, On enables them, Toggle switches between on and off on each press."
		);
		GyroCalibrateButton = gyroCategory.CreateEntryEnum("GyroCalibrateButton", ControllerButton.Touchpad,
			"Controller button to calibrate gyro. If you experience gyro drift, place the controller on a flat surface and hold down this button for at least a second to calibrate."
		);
		GyroSensitivity = gyroCategory.CreateEntry("Sensitivity", 3f,
			"Gyro sensitivity. 1 means a 90 degrees turn rotates the camera 90 degrees, 2 means a 90 degree turn rotates the camera 180 degrees."
		);
		GyroSensitivityRatio = gyroCategory.CreateEntry("SensitivityRatio", 1f,
			"Vertical gyro sensitivity multiplier. 0.75 means vertical sensitivity is 75% smaller than horizontal sensitivity."
		);
		GyroTightening = gyroCategory.CreateEntry("Tightening", 1f,
			"Squishes gyro rotations below this speed (in degrees per second) down to 0. Works as a soft deadzone to reduce the effects of shaky hands."
		);
		GyroSmoothingThreshold = gyroCategory.CreateEntry("SmoothingThreshold", 25f,
			"Smoothes gyro rotations below this speed (in degrees per second), but doesn't smooth rotations above it. Helps to reduce the effects of shaky hands."
		);
		GyroSmoothingTime = gyroCategory.CreateEntry("SmoothingTime", 0.1f,
			"Amount of time used for smoothing. Higher values increase the delay between your real movements and in game movements."
		);
		GyroDisableWhenWalking = gyroCategory.CreateEntry("DisableWhenWalking", false,
			"Wether to disable gyro while slow walking."
		);
		GyroLevelStartPauseLength = gyroCategory.CreateEntry("LevelStartPauseLength", 0.2f,
			"Amount of time to pause Gyro for at the start of each level. This prevents accidentally looking away."
		);

		SticksDeadzones = sticksCategory.CreateEntry("Deadzones", 0.125f,
			"Increase deadzones if you experience stick drift. Decrease them for higher precision aiming, if your controller allows it."
		);
		SticksPowerCurve = sticksCategory.CreateEntry("CameraPowerCurve", 1f,
			"Squish or stretch your right stick movements based on this value. Values above 1 squish motion closer to the center of the stick, values below 1 stretch them closer to the edge."
		);
		SticksDegreesPerSecond = sticksCategory.CreateEntry("CameraDegreesPerSecond", 180f,
			"Right stick sensitivity. 180 means it takes 2 seconds while holding the stick all the way to the side to do a full rotation."
		);
		SticksSensitivityRatio = sticksCategory.CreateEntry("CameraSensitivityRatio", 0.75f,
			"Vertical right stick sensitivity multiplier. 0.75 means vertical sensitivity is 75% smaller than horizontal sensitivity."
		);

		AutoLookHorEnabled = autoLookCategory.CreateEntry("HorizontalEnabled", true,
			"Enable automatic horizontal rotation of the camera to face the direction of the left stick."
		);
		AutoLookVerEnabled = autoLookCategory.CreateEntry("VerticalEnabled", true,
			"Enable automatic vertical rotation of the camera to match your vertical velocity. Jumping tilts the camera up, falling tilts the camera down."
		);
		AutoLookHorSpeed = autoLookCategory.CreateEntry("HorizontalSpeed", 1f,
			"How fast horizontal auto look reacts to changes."
		);
		AutoLookHorStrength = autoLookCategory.CreateEntry("HorizontalStrength", 1f,
			"How strong the effects of horizontal auto look are."
		);
		AutoLookVerSpeed = autoLookCategory.CreateEntry("VerticalSpeed", 1f,
			"How fast horizontal auto look reacts to changes."
		);
		AutoLookVerStrength = autoLookCategory.CreateEntry("VerticalStrength", 1f,
			"How strong the effects of vertical auto look are."
		);
		AutoLookVerBaseAngle = autoLookCategory.CreateEntry("VerticalBaseAngle", 1f,
			"Default vertical angle while touching the ground. Higher values tilt the camera downward."
		);

		RumbleIntensity = rumbleCategory.CreateEntry("RumbleIntensity", 1f,
			"Overall rumble intensity. Set to 0 to completely disable rumble"
		);
		RumbleOnDamage = rumbleCategory.CreateEntry("RumbleOnDamage", 1f,
			"Rumble intensity when taking damage."
		);
		RumbleOnLand = rumbleCategory.CreateEntry("RumbleOnLand", 1f,
			"Rumble intensity when landing. Different landing grades have different rumble properties."
		);
		RumbleOnFastRun = rumbleCategory.CreateEntry("RumbleOnFastRun", 1f,
			"Rumble intensity when charging up fast run."
		);
		RumbleOnSparkPickup = rumbleCategory.CreateEntry("RumbleOnSparkPickup", 1f,
			"Rumble intensity when picking up sparks."
		);
		RumbleOnBoostRing = rumbleCategory.CreateEntry("RumbleOnBoostRing", 1f,
			"Rumble intensity when going through a boost ring."
		);
		RumbleOnBoardBoost = rumbleCategory.CreateEntry("RumbleOnBoardBoost", 1f,
			"Rumble intensity when board boosting."
		);
		RumbleOnGrapple = rumbleCategory.CreateEntry("RumbleOnGrapple", 1f,
			"Rumble intensity when using the grappling hook ability or item."
		);
		RumbleOnFly = rumbleCategory.CreateEntry("RumbleOnFly", 1f,
			"Rumble intensity when activating Sage's Cowl."
		);
	}

	public void UpdateConfig(Config config)
	{
		config.GeneralDisableAbilitiesInSafeZones = GeneralDisableAbilitiesInSafeZones.Value;

		config.GyroEnabled = GyroEnabled.Value;
		config.GyroSpace = GyroSpace.Value;
		config.GyroButton = GyroButton.Value;
		config.GyroButtonMode = GyroButtonMode.Value;
		config.GyroCalibrateButton = GyroCalibrateButton.Value;
		config.GyroSensitivity = GyroSensitivity.ValueFloat;
		config.GyroSensitivityRatio = GyroSensitivityRatio.ValueFloat;
		config.GyroTightening = GyroTightening.ValueFloat;
		config.GyroSmoothingThreshold = GyroSmoothingThreshold.ValueFloat;
		config.GyroSmoothingTime = GyroSmoothingTime.ValueFloat;
		config.GyroDisableWhenWalking = GyroDisableWhenWalking.Value;
		config.GyroLevelStartPauseLength = GyroLevelStartPauseLength.ValueFloat;

		config.SticksDeadzones = SticksDeadzones.ValueFloat;
		config.SticksPowerCurve = SticksPowerCurve.ValueFloat;
		config.SticksDegreesPerSecond = SticksDegreesPerSecond.ValueFloat;
		config.SticksSensitivityRatio = SticksSensitivityRatio.ValueFloat;

		config.AutoLookHorEnabled = AutoLookHorEnabled.Value;
		config.AutoLookVerEnabled = AutoLookVerEnabled.Value;
		config.AutoLookHorSpeed = AutoLookHorSpeed.ValueFloat;
		config.AutoLookHorStrength = AutoLookHorStrength.ValueFloat;
		config.AutoLookVerSpeed = AutoLookVerSpeed.ValueFloat;
		config.AutoLookVerStrength = AutoLookVerStrength.ValueFloat;
		config.AutoLookVerBaseAngle = AutoLookVerBaseAngle.ValueFloat;

		config.RumbleIntensity = RumbleIntensity.ValueFloat;
		config.RumbleOnDamage = RumbleOnDamage.ValueFloat;
		config.RumbleOnLand = RumbleOnLand.ValueFloat;
		config.RumbleOnFastRun = RumbleOnFastRun.ValueFloat;
		config.RumbleOnSparkPickup = RumbleOnSparkPickup.ValueFloat;
		config.RumbleOnBoostRing = RumbleOnBoostRing.ValueFloat;
		config.RumbleOnBoardBoost = RumbleOnBoardBoost.ValueFloat;
		config.RumbleOnGrapple = RumbleOnGrapple.ValueFloat;
		config.RumbleOnFly = RumbleOnFly.ValueFloat;
	}

	public static LandfallConfig LoadFromFile(string path)
	{
		IniData? ini;
		if (File.Exists(path))
		{
			ini = iniParser.Parse(File.ReadAllText(path));
		}
		else
		{
			ini = new();
		}

		return new LandfallConfig(new IniConfigFile(ini));
	}

	public void SaveToFile(string path)
	{
		string newIni = iniFormatter.Format(file.IniData, iniFormattingConfiguration);
		File.WriteAllText(path, newIni);
	}
}