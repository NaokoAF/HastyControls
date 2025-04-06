using HarmonyLib;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public static class HastySettings
{
	const string Category = ModInfo.Name;
	static readonly IEnumerable<string> GyroSpaceChoices = ["Local (Yaw)", "Local (Roll)", "Player (Turn)", "Player (Lean)"];
	static readonly IEnumerable<string> GyroButtonModeChoices = ["Disable Gyro while held", "Enable Gyro while held", "Toggle Gyro when pressed"];

	public class GeneralDisableAbilitiesInSafeZonesSetting() : HastyBoolSetting(Category, "Disable Abilities in Safe Zones", true);
	public class GamepadSensitivityRatioSetting() : HastyFloatSetting(Category, "Gamepad Sensitivity Ratio", 0f, 5f, 1f);
	public class GamepadPowerCurveSetting() : HastyFloatSetting(Category, "Gamepad Power Curve", 0.01f, 5f, 1f);
	public class GamepadDeadzonesSetting() : HastyFloatSetting(Category, "Gamepad Deadzones", 0.01f, 1f, 0.125f);
	public class GyroSensitivitySetting() : HastyFloatSetting(Category, "Gyro Sensitivity", 0f, 10f, 3f);
	public class GyroSensitivityRatioSetting() : HastyFloatSetting(Category, "Gyro Sensitivity Ratio", 0f, 5f, 0.75f);
	public class GyroDisableWhenWalkingSetting() : HastyBoolSetting(Category, "Gyro Disable when Walking", false);
	public class GyroSpaceSetting() : HastyEnumSetting<GyroSpace>(Category, "Gyro Space", GyroSpace.PlayerTurn, GyroSpaceChoices);
	public class GyroButtonSetting() : HastyEnumSetting<ControllerButton>(Category, "Gyro Button", ControllerButton.RightStick);
	public class GyroButtonModeSetting() : HastyEnumSetting<GyroButtonMode>(Category, "Gyro Button Mode", GyroButtonMode.Off, GyroButtonModeChoices);
	public class GyroCalibrateButtonSetting() : HastyEnumSetting<ControllerButton>(Category, "Gyro Calibrate Button", ControllerButton.Touchpad);
	public class GyroTighteningSetting() : HastyFloatSetting(Category, "Gyro Tightening", 0f, 15f, 3f);
	public class GyroSmoothingThresholdSetting() : HastyFloatSetting(Category, "Gyro Smoothing Threshold", 0f, 300f, 35f);
	public class GyroSmoothingTimeSetting() : HastyFloatSetting(Category, "Gyro Smoothing Time", 0f, 0.5f, 0.1f);
	public class AutoLookHorSpeedSetting() : HastyFloatSetting(Category, "AutoLook Horizontal Speed", 0f, 10f, 1f);
	public class AutoLookHorStrengthSetting() : HastyFloatSetting(Category, "AutoLook Horizontal Strength", 0f, 10f, 1f);
	public class AutoLookVerSpeedSetting() : HastyFloatSetting(Category, "AutoLook Vertical Speed", 0f, 10f, 1f);
	public class AutoLookVerStrengthSetting() : HastyFloatSetting(Category, "AutoLook Vertical Strength", 0f, 10f, 1f);
	public class AutoLookVerBaseAngleSetting() : HastyFloatSetting(Category, "AutoLook Vertical Base Angle", -5f, 5f, 1f);
	public class RumbleIntensitySetting() : HastyFloatSetting(Category, "Rumble Intensity", 0f, 5f, 1f);
	public class RumbleOnDamageSetting() : HastyFloatSetting(Category, "Rumble on Damage Intensity", 0f, 5f, 1f);
	public class RumbleOnLandSetting() : HastyFloatSetting(Category, "Rumble on Land Intensity", 0f, 5f, 1f);
	public class RumbleOnFastRunSetting() : HastyFloatSetting(Category, "Rumble on Fast Run Intensity", 0f, 5f, 1f);
	public class RumbleOnSparkPickupSetting() : HastyFloatSetting(Category, "Rumble on Spark Pickup Intensity", 0f, 5f, 1f);
	public class RumbleOnBoostRingSetting() : HastyFloatSetting(Category, "Rumble on Boost Ring Intensity", 0f, 5f, 1f);
	public class RumbleOnBoardBoostSetting() : HastyFloatSetting(Category, "Rumble on Board Boost Intensity", 0f, 5f, 1f);
	public class RumbleOnGrappleSetting() : HastyFloatSetting(Category, "Rumble on Grapple Intensity", 0f, 5f, 1f);
	public class RumbleOnFlySetting() : HastyFloatSetting(Category, "Rumble on Poncho Intensity", 0f, 5f, 1f);

	static AccessTools.FieldRef<HasteSettingsHandler, List<Setting>> settingsRef = AccessTools.FieldRefAccess<HasteSettingsHandler, List<Setting>>("settings");
	static AccessTools.FieldRef<HasteSettingsHandler, ISettingsSaveLoad> settingsSaveLoadRef = AccessTools.FieldRefAccess<HasteSettingsHandler, ISettingsSaveLoad>("_settingsSaveLoad");

	static HasteSettingsHandler Handler => GameHandler.Instance.SettingsHandler;

	public static void Initialize()
	{
		AddCategory(Category, $"<size=65%>{Category}");
		Add<GeneralDisableAbilitiesInSafeZonesSetting>();
		Add<GamepadSensitivityRatioSetting>();
		Add<GamepadPowerCurveSetting>();
		Add<GamepadDeadzonesSetting>();
		Add<GyroSensitivitySetting>();
		Add<GyroSensitivityRatioSetting>();
		Add<GyroDisableWhenWalkingSetting>();
		Add<GyroSpaceSetting>();
		Add<GyroButtonSetting>();
		Add<GyroButtonModeSetting>();
		Add<GyroCalibrateButtonSetting>();
		Add<GyroTighteningSetting>();
		Add<GyroSmoothingThresholdSetting>();
		Add<GyroSmoothingTimeSetting>();
		Add<AutoLookHorSpeedSetting>();
		Add<AutoLookHorStrengthSetting>();
		Add<AutoLookVerSpeedSetting>();
		Add<AutoLookVerStrengthSetting>();
		Add<AutoLookVerBaseAngleSetting>();
		Add<RumbleIntensitySetting>();
		Add<RumbleOnDamageSetting>();
		Add<RumbleOnLandSetting>();
		Add<RumbleOnFastRunSetting>();
		Add<RumbleOnSparkPickupSetting>();
		Add<RumbleOnBoostRingSetting>();
		Add<RumbleOnBoardBoostSetting>();
		Add<RumbleOnGrappleSetting>();
		Add<RumbleOnFlySetting>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetSetting<T>() where T : Setting, new() => SettingsStorage<T>.Setting;

	static void AddCategory(string key, string name)
	{
		SettingsUIPage.LocalizedTitles.Add(key, new(ModInfo.Guid, name));
	}

	static void Add<T>() where T : Setting, new()
	{
		var setting = SettingsStorage<T>.Setting;
		settingsRef(Handler).Add(setting);
		setting.Load(settingsSaveLoadRef(Handler));
		setting.ApplyValue();
	}

	static class SettingsStorage<T> where T : Setting, new()
	{
		public static T Setting = new();
	}
}