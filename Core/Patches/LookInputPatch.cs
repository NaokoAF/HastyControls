using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

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
					newMag = MathF.Pow(newMag, Mod.Config.SticksPowerCurve);

				lookInput = lookInput / mag * newMag;
			}

			// get sensitivity
			float gamepadSensitivity = Mod.Config.SticksDegreesPerSecond;
			if (gamepadSensitivity == 0f)
				gamepadSensitivity = (gamepadSensitivitySettingRef(character)?.Value ?? 1f) * 90f;

			___lookInput = lookInput * gamepadSensitivity * Time.unscaledDeltaTime;
			___lookInput.y *= Mod.Config.SticksSensitivityRatio;
		}
		else
		{
			float mouseSensitivity = mouseSensitivitySettingRef(character)?.Value ?? 1f;
			___lookInput = lookInput * mouseSensitivity;
		}

		if (!Mod.Config.GyroEnabled || Mod.ControllerManager == null)
			return;

		// apply gyro
		var gyro = Mod.ControllerManager.GyroDelta * Mathf.Rad2Deg;
		(gyro.X, gyro.Y) = (gyro.Y, gyro.X); // swap axes

		gyro *= Mod.Config.GyroSensitivity;
		gyro.Y *= Mod.Config.GyroSensitivityRatio;
		___lookInput.x -= gyro.X;
		___lookInput.y -= gyro.Y;
	}
}
