using HarmonyLib;
using Landfall.Haste;
using System.Runtime.CompilerServices;
using UnityEngine.Localization;
using Zorro.Settings;

namespace HastyControls.Core.Settings;

public static class HastySettings
{
	const string Category = ModInfo.Name;
	static readonly IEnumerable<string> GyroSpaceChoices = ["Local Yaw (Handhelds)", "Local Roll (Handhelds)", "Player Turn (Recommended)", "Player Lean (Recommended)"];
	static readonly IEnumerable<string> GyroButtonModeChoices = ["Disable Gyro while held", "Enable Gyro while held", "Toggle Gyro when pressed"];

	public class GeneralCollapsibleSetting() : HastyCollapsibleSetting(Category, "General Settings", "");
	public class GamepadCollapsibleSetting() : HastyCollapsibleSetting(Category, "Gamepad Settings", "");
	public class GyroCollapsibleSetting() : HastyCollapsibleSetting(Category, "Gyro Settings", "");
	public class AutoLookCollapsibleSetting() : HastyCollapsibleSetting(Category, "AutoLook Settings", "");
	public class RumbleCollapsibleSetting() : HastyCollapsibleSetting(Category, "Rumble Settings", "");

	public class GeneralDisableAbilitiesInSafeZonesSetting() : HastyBoolSetting(Category, "Disable Abilities in Safe Zones", "Prevent accidental usage of abilities in the shop and healing areas.", true);
	public class GamepadSensitivityRatioSetting() : HastyFloatSetting(Category, "Gamepad Sensitivity Ratio", "Vertical sensitivity multiplier. 0.75 means vertical sensitivity is 75% slower than horizontal.", 0f, 5f, 1f);
	public class GamepadPowerCurveSetting() : HastyFloatSetting(Category, "Gamepad Power Curve", "Right stick power curve. Values above 1 squish motion closer to the center of the stick, values below 1 stretch them closer to the edge.", 0.01f, 5f, 1f);
	public class GamepadDeadzonesSetting() : HastyFloatSetting(Category, "Gamepad Deadzones", "Adjust deadzones to combat stick drift.", 0.01f, 1f, 0.125f);
	public class GyroSensitivitySetting() : HastyFloatSetting(Category, "Gyro Sensitivity", "Gyro sensitivity. 2 means a 90 degree turn rotates the camera 180 degrees.", 0f, 10f, 3f);
	public class GyroSensitivityRatioSetting() : HastyFloatSetting(Category, "Gyro Sensitivity Ratio", "Vertical sensitivity multiplier. 0.75 means vertical sensitivity is 75% slower than horizontal.", 0f, 5f, 0.75f);
	public class GyroDisableWhenWalkingSetting() : HastyBoolSetting(Category, "Gyro Disable when Walking", "Disable gyro while slow walking.", false);
	public class GyroSpaceSetting() : HastyEnumSetting<GyroSpace>(Category, "Gyro Space", "Algorithm used to convert real world movements to the in game camera.", GyroSpace.PlayerTurn, GyroSpaceChoices);
	public class GyroButtonSetting() : HastyEnumSetting<ControllerButton>(Category, "Gyro Button", "Controller button to enable/disable gyro.", ControllerButton.RightStick);
	public class GyroButtonModeSetting() : HastyEnumSetting<GyroButtonMode>(Category, "Gyro Button Mode", "Behavior of gyro button.", GyroButtonMode.Off, GyroButtonModeChoices);
	public class GyroCalibrateButtonSetting() : HastyEnumSetting<ControllerButton>(Category, "Gyro Calibrate Button", "Button to correct gyro drift.", ControllerButton.Touchpad);
	public class GyroTighteningSetting() : HastyFloatSetting(Category, "Gyro Tightening", "Soft gyro deadzone to reduce the effects of shaky hands.", 0f, 15f, 3f);
	public class GyroSmoothingThresholdSetting() : HastyFloatSetting(Category, "Gyro Smoothing Threshold", "Threshold below which gyro rotations are smoothed.", 0f, 300f, 35f);
	public class GyroSmoothingTimeSetting() : HastyFloatSetting(Category, "Gyro Smoothing Time", "Amount of time gyro is smoothed for.", 0f, 0.5f, 0.1f);
	public class AutoLookHorSpeedSetting() : HastyFloatSetting(Category, "AutoLook Horizontal Speed", "How fast the camera faces the direction of the left stick.", 0f, 10f, 1f);
	public class AutoLookHorStrengthSetting() : HastyFloatSetting(Category, "AutoLook Horizontal Strength", "How strongly the camera faces the direction of the left stick.", 0f, 10f, 1f);
	public class AutoLookVerSpeedSetting() : HastyFloatSetting(Category, "AutoLook Vertical Speed", "How fast the camera follows your vertical velocity.", 0f, 10f, 1f);
	public class AutoLookVerStrengthSetting() : HastyFloatSetting(Category, "AutoLook Vertical Strength", "How strongly the camera follows your vertical velocity.", 0f, 10f, 1f);
	public class AutoLookVerBaseAngleSetting() : HastyFloatSetting(Category, "AutoLook Vertical Base Angle", "Default vertical angle while touching the ground. Higher values tilt the camera downward.", -5f, 5f, 1f);
	public class RumbleIntensitySetting() : HastyFloatSetting(Category, "Rumble Intensity", "Overall rumble intensity. 0 to disable rumble completely.", 0f, 5f, 1f);
	public class RumbleOnDamageSetting() : HastyFloatSetting(Category, "Rumble on Damage Intensity", "Rumble intensity when taking damage.", 0f, 5f, 1f);
	public class RumbleOnLandSetting() : HastyFloatSetting(Category, "Rumble on Land Intensity", "Rumble intensity when landing. Different landing grades have different rumble properties.", 0f, 5f, 1f);
	public class RumbleOnFastRunSetting() : HastyFloatSetting(Category, "Rumble on Fast Run Intensity", "Rumble intensity when charging up fast run.", 0f, 5f, 1f);
	public class RumbleOnSparkPickupSetting() : HastyFloatSetting(Category, "Rumble on Spark Pickup Intensity", "Rumble intensity when picking up sparks.", 0f, 5f, 1f);
	public class RumbleOnBoostRingSetting() : HastyFloatSetting(Category, "Rumble on Boost Ring Intensity", "Rumble intensity when going through a boost ring.", 0f, 5f, 1f);
	public class RumbleOnBoardBoostSetting() : HastyFloatSetting(Category, "Rumble on Board Boost Intensity", "Rumble intensity when board boosting.", 0f, 5f, 1f);
	public class RumbleOnGrappleSetting() : HastyFloatSetting(Category, "Rumble on Grapple Intensity", "Rumble intensity when activating the grappling hook (Heir's Javelin).", 0f, 5f, 1f);
	public class RumbleOnFlySetting() : HastyFloatSetting(Category, "Rumble on Poncho Intensity", "Rumble intensity when activating the poncho (Sage's Cowl).", 0f, 5f, 1f);

	static AccessTools.FieldRef<HasteSettingsHandler, List<Setting>> settingsRef = AccessTools.FieldRefAccess<HasteSettingsHandler, List<Setting>>("settings");
	static AccessTools.FieldRef<HasteSettingsHandler, ISettingsSaveLoad> settingsSaveLoadRef = AccessTools.FieldRefAccess<HasteSettingsHandler, ISettingsSaveLoad>("_settingsSaveLoad");
	static List<IHastySetting> hastySettings = new();

	public static void Initialize()
	{
		var general = Add<GeneralCollapsibleSetting>();
		Add<GeneralDisableAbilitiesInSafeZonesSetting>(general);

		var gamepad = Add<GamepadCollapsibleSetting>();
		Add<GamepadSensitivityRatioSetting>(gamepad);
		Add<GamepadPowerCurveSetting>(gamepad);
		Add<GamepadDeadzonesSetting>(gamepad);

		var gyro = Add<GyroCollapsibleSetting>();
		Add<GyroSensitivitySetting>(gyro);
		Add<GyroSensitivityRatioSetting>(gyro);
		Add<GyroDisableWhenWalkingSetting>(gyro);
		Add<GyroSpaceSetting>(gyro);
		Add<GyroButtonSetting>(gyro);
		Add<GyroButtonModeSetting>(gyro);
		Add<GyroCalibrateButtonSetting>(gyro);
		Add<GyroTighteningSetting>(gyro);
		Add<GyroSmoothingThresholdSetting>(gyro);
		Add<GyroSmoothingTimeSetting>(gyro);

		var autolook = Add<AutoLookCollapsibleSetting>();
		Add<AutoLookHorSpeedSetting>(autolook);
		Add<AutoLookHorStrengthSetting>(autolook);
		Add<AutoLookVerSpeedSetting>(autolook);
		Add<AutoLookVerStrengthSetting>(autolook);
		Add<AutoLookVerBaseAngleSetting>(autolook);

		var rumble = Add<RumbleCollapsibleSetting>();
		Add<RumbleIntensitySetting>(rumble);
		Add<RumbleOnDamageSetting>(rumble);
		Add<RumbleOnLandSetting>(rumble);
		Add<RumbleOnFastRunSetting>(rumble);
		Add<RumbleOnSparkPickupSetting>(rumble);
		Add<RumbleOnBoostRingSetting>(rumble);
		Add<RumbleOnBoardBoostSetting>(rumble);
		Add<RumbleOnGrappleSetting>(rumble);
		Add<RumbleOnFlySetting>(rumble);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetSetting<T>() where T : Setting, new() => SettingsStorage<T>.Setting;

	static void AddCategory(string key, string name)
	{
		SettingsUIPage.LocalizedTitles.Add(key, new UnlocalizedString(name));
	}

	static T Add<T>(HastyCollapsibleSetting? collapsibleCategory = null) where T : Setting, IHastySetting, new()
	{
		var setting = SettingsStorage<T>.Setting;
		hastySettings.Add(setting);

		// if a collapsible is provided, only show the setting if it isn't collapsed, and the collapsible setting is also visible
		if (collapsibleCategory != null)
		{
			setting.ShowCondition = () => !collapsibleCategory.Collapsed && (collapsibleCategory.ShowCondition?.Invoke() ?? true);
		}

		var handler = GameHandler.Instance.SettingsHandler;
		settingsRef(handler).Add(setting);
		setting.Load(settingsSaveLoadRef(handler));
		setting.ApplyValue();
		return setting;
	}

	internal static LocalizedString CreateDisplayName(string name, string description)
	{
		return new UnlocalizedString($"{name}\n<size=60%><alpha=#50>{description}");
	}

	static class SettingsStorage<T> where T : Setting, new()
	{
		public static T Setting = new();
	}
}