using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(PlayerCharacter.PlayerInput))]
internal static class LookInputPatch
{
	static AccessTools.FieldRef<PlayerCharacter, GamepadSensitivitySetting> gamepadSensitivitySettingRef = AccessTools.FieldRefAccess<PlayerCharacter, GamepadSensitivitySetting>("gamepadSensitivitySetting");
	static AccessTools.FieldRef<PlayerCharacter, MouseSensitivitySetting> mouseSensitivitySettingRef = AccessTools.FieldRefAccess<PlayerCharacter, MouseSensitivitySetting>("mouseSensitivitySetting");

	[HarmonyPatch(nameof(PlayerCharacter.PlayerInput.SampleInput))]
	[HarmonyPostfix]
	static void SampleInputPostfix(PlayerCharacter character, ref Vector2 ___lookInput)
	{
		Vector2 lookInput = HasteInputSystem.Look.GetValue();
		if (HasteInputSystem.Look.Action.activeControl?.device is Gamepad)
		{
			// remap deadzone and apply power curve
			float mag = lookInput.magnitude;
			if (mag > 0f)
			{
				float min = InputSystem.settings.defaultDeadzoneMin;
				float max = InputSystem.settings.defaultDeadzoneMax;
				float newMag = Math.Clamp((mag - min) / (max - min), 0f, 1f);
				// apply power curve
				if (newMag > 0f)
					newMag = MathF.Pow(newMag, GetSetting<GamepadPowerCurveSetting>().Value);

				lookInput = lookInput / mag * newMag;
			}

			// get sensitivity
			float gamepadSensitivity = (gamepadSensitivitySettingRef(character)?.Value ?? 1f) * 90f;
			___lookInput = lookInput * gamepadSensitivity * Time.unscaledDeltaTime;
			___lookInput.y *= GetSetting<GamepadSensitivityRatioSetting>().Value;
		}
		else
		{
			float mouseSensitivity = mouseSensitivitySettingRef(character)?.Value ?? 1f;
			___lookInput = lookInput * mouseSensitivity;
		}

		if (GetSetting<GyroSensitivitySetting>().Value == 0f || Mod.ControllerManager == null)
			return;

		// apply gyro
		var gyro = Mod.ControllerManager.GyroDelta * Mathf.Rad2Deg;
		(gyro.X, gyro.Y) = (gyro.Y, gyro.X); // swap axes

		gyro *= GetSetting<GyroSensitivitySetting>().Value;
		gyro.Y *= GetSetting<GyroSensitivityRatioSetting>().Value;

		if (GetSetting<GyroInvertXSetting>().Value) gyro.X = -gyro.X;
		if (GetSetting<GyroInvertYSetting>().Value) gyro.Y = -gyro.Y;

		___lookInput.x -= gyro.X;
		___lookInput.y -= gyro.Y;
	}
}
