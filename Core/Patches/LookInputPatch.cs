using UnityEngine;
using UnityEngine.InputSystem;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

internal class LookInputPatch : IHastyPatch
{
	private MouseSensitivitySetting? mouseSensitivitySetting;
	private GamepadSensitivitySetting? gamepadSensitivitySetting;

	public void Patch(HastyControlsMod mod)
	{
		On.PlayerCharacter.PlayerInput.SampleInput += (orig, self, character, autoRun) =>
		{
			orig(self, character, autoRun);

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
				gamepadSensitivitySetting ??= GameHandler.Instance.SettingsHandler.GetSetting<GamepadSensitivitySetting>();
				lookInput *= gamepadSensitivitySetting.Value * 90f * Time.unscaledDeltaTime;
				lookInput.y *= GetSetting<GamepadSensitivityRatioSetting>().Value;
			}
			else
			{
				mouseSensitivitySetting ??= GameHandler.Instance.SettingsHandler.GetSetting<MouseSensitivitySetting>();
				lookInput *= mouseSensitivitySetting.Value;
			}

			if (GetSetting<GyroSensitivitySetting>().Value != 0f)
			{
				// apply gyro
				var gyro = mod.ControllerManager.GyroDelta * Mathf.Rad2Deg;
				(gyro.X, gyro.Y) = (gyro.Y, gyro.X); // swap axes

				gyro *= GetSetting<GyroSensitivitySetting>().Value;
				gyro.Y *= GetSetting<GyroSensitivityRatioSetting>().Value;

				if (GetSetting<GyroInvertXSetting>().Value) gyro.X = -gyro.X;
				if (GetSetting<GyroInvertYSetting>().Value) gyro.Y = -gyro.Y;

				lookInput.x -= gyro.X;
				lookInput.y -= gyro.Y;
			}

			self.lookInput = lookInput;
		};
	}
}