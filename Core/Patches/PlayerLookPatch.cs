using System.Runtime.CompilerServices;
using UnityEngine;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

internal class PlayerLookPatch : IHastyPatch
{
	HastyControlsMod? mod;
	
	bool gyroResetPrevButtonDown;
	float gyroResetT;
	Vector2 gyroResetAngle;
	Vector2 gyroResetPrevAngle;
	
	public void Patch(HastyControlsMod mod)
	{
		this.mod = mod;
		
		On.PlayerCharacter.SetLook += (orig, self) =>
		{
			if (!HasteInputSystem.CanTakeInput())
				return;

			// alter rotation
			DoAutoLook(self);
			DoGyroReset(self);

			// rotate camera and clamp
			var data = self.data;
			data.lookRotationValues.x += self.input.lookInput.x;
			data.lookRotationValues.y += self.input.lookInput.y;
			data.lookRotationValues.y = Mathf.Clamp(data.lookRotationValues.y, -80f, 80f);

			// set helper values
			data.lookRotationEulerAngles = new Vector3(data.lookRotationValues.y, data.lookRotationValues.x, 0f);
			data.lookDir = Quaternion.Euler(data.lookRotationEulerAngles) * Vector3.forward;
		};
	}
	
	private void DoAutoLook(PlayerCharacter character)
	{
		var data = character.data;
		var input = character.input;
		var refs = character.refs;
		var config = character.config;

		// auto look
		if (character.IsVisible && data.movementType == MovementType.Fast)
		{
			float horizontalSpeed = GetSetting<AutoLookHorSpeedSetting>().Value;
			float horizontalStrength = GetSetting<AutoLookHorStrengthSetting>().Value;
			if (horizontalSpeed != 0f && horizontalStrength != 0f)
			{
				// local-space point in front of player
				Vector3 direction = refs.playerVisualRotation.visual.transform.InverseTransformPoint(refs.playerVisualRotation.visual.transform.position + data.lookDir);

				// smoothed magnitude value based on movement and look input horizontal magnitudes
				// this makes it so the look cancels out the movement input, sort of
				// NOTE: i have absolutely no clue why this is clamped to -1 instead of 0, but that's how the game does it
				float autoLookTarget = Mathf.Clamp(Mathf.Abs(input.movementInput.x) - Mathf.Abs(input.lookInput.x), -1f, 1f);
				data.autoLookValue = Mathf.Lerp(data.autoLookValue, autoLookTarget, Time.deltaTime * 2f * horizontalSpeed);

				// rotate
				float horizontalDelta = -direction.x * Mathf.Clamp01(data.autoLookValue) * config.movementLookSpring * Time.deltaTime;
				horizontalDelta *= horizontalStrength;
				data.lookRotationValues.x += horizontalDelta;
			}

			float verticalSpeed = GetSetting<AutoLookVerSpeedSetting>().Value;
			float verticalStrength = GetSetting<AutoLookVerStrengthSetting>().Value;
			if (verticalSpeed != 0f && verticalStrength != 0f)
			{
				float verticalBaseAngle = GetSetting<AutoLookVerBaseAngleSetting>().Value;
				// caculate angle based on velocity's vertical direction
				float verticalTarget = (verticalBaseAngle * 20f) - (refs.rig.linearVelocity.normalized.y * verticalStrength * 20f);

				// tilt the camera by a set value when holding down the fast fall button
				if (data.fastFalling && !data.mostlyGrounded)
					verticalTarget += verticalStrength * 20f;

				// smoothly interpolate between the current rotation and the target
				data.lookRotationValues.y = Mathf.Lerp(data.lookRotationValues.y, verticalTarget, Time.deltaTime * verticalSpeed);
			}
		}
	}

	private void DoGyroReset(PlayerCharacter character)
	{
		GyroButtonMode mode = GetSetting<GyroButtonModeSetting>().Value;
		if (mode != GyroButtonMode.Recenter && mode != GyroButtonMode.RecenterAndOff)
			return;

		var data = character.data;

		// activate
		bool gyroButtonDown = mod!.ControllerManager!.GyroButtonDown;
		if (gyroButtonDown && !gyroResetPrevButtonDown)
		{
			gyroResetT = 1f;
			gyroResetPrevAngle = Vector2.zero;

			// reset vertically
			gyroResetAngle.y = 15f - data.lookRotationValues.y;

			// look towards velocity
			if (data.velocityMagnitude > 0.1f)
			{
				float horAngle = Mathf.Atan2(data.velocity.x, data.velocity.z) * Mathf.Rad2Deg;
				gyroResetAngle.x = Mathf.DeltaAngle(data.lookRotationValues.x, horAngle);
			}
			else
			{
				gyroResetAngle.x = 0f;
			}
		}
		gyroResetPrevButtonDown = gyroButtonDown;

		// animate
		if (gyroResetT > 0f)
		{
			gyroResetT -= Time.deltaTime / 0.2f;
			gyroResetT = Math.Max(gyroResetT, 0f);

			Vector2 angle = gyroResetAngle * EaseOutCubic(1f - gyroResetT);
			Vector2 angleDelta = angle - gyroResetPrevAngle;
			gyroResetPrevAngle = angle;

			data.lookRotationValues += angleDelta;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float EaseOutCubic(float t)
	{
		t -= 1f;
		return 1 + t * t * t;
	}
}
