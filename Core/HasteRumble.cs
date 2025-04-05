using UnityEngine;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

public class HasteRumble
{
	HasteEvents events;
	ControllerManager controllers;

	public HasteRumble(HasteEvents events, ControllerManager controllers)
	{
		this.events = events;
		this.controllers = controllers;

		events.PlayerDamaged += OnPlayerDamaged;
		events.PlayerLanded += OnPlayerLanded;
		events.PlayerCharging += OnPlayerCharging;
		events.PlayerChargingEnded += OnPlayerChargingEnded;
		events.PlayerSparkPickedUp += OnPlayerSparkPickedUp;
		events.PlayerBoostRingPassed += OnPlayerBoostRingPassed;
		events.PlayerBoardBoosting += OnPlayerBoardBoosting;
		events.PlayerFlyAbilityUsed += OnPlayerFlyAbilityUsed;
		events.PlayerGrappleAbilityUsed += OnPlayerGrappleAbilityUsed;
		events.PlayerGrappleAbilityFinished += OnPlayerGrappleAbilityFinished;
	}

	private void OnPlayerDamaged(float damage, Transform dealer, EffectSource source)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnDamageSetting>().Value;
		if (multiplier <= 0) return;

		var player = Player.localPlayer;
		if (player != null)
		{
			float maxHealth = player.stats.maxHealth.baseValue * player.stats.maxHealth.multiplier;
			if (maxHealth == 0) return;

			float damageRatio = Mathf.Clamp01(damage / maxHealth);
			controllers.ActiveController?.Rumble(damageRatio * multiplier, 0f, 0.2f);
		}
	}

	private void OnPlayerLanded(LandingType type, bool arg2)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnLandSetting>().Value;
		if (multiplier <= 0) return;

		float low;
		float high;
		float duration;
		switch (type)
		{
			case LandingType.Bad:
				low = 0.3f;
				high = 0f;
				duration = 0.2f;
				break;
			case LandingType.Ok:
				low = 0.05f;
				high = 1f;
				duration = 0.1f;
				break;
			case LandingType.Good:
				low = 0.05f;
				high = 1f;
				duration = 0.125f;
				break;
			case LandingType.Perfect:
				low = 1f;
				high = 1f;
				duration = 0.125f;
				break;
			default:
				return;
		}

		controllers.ActiveController?.Rumble(low * multiplier, high * multiplier, duration);
	}

	private void OnPlayerCharging(float amount, bool up)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFastRunSetting>().Value;
		if (multiplier <= 0) return;

		if (up)
		{
			amount = Mathf.Clamp01(amount);
			controllers.ActiveController?.Rumble(
				amount * 0.125f * multiplier,
				amount * 0.125f * multiplier,
				0.1f
			);
		}
	}

	private void OnPlayerChargingEnded()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFastRunSetting>().Value;
		if (multiplier <= 0) return;

		controllers.ActiveController?.Rumble(1f * multiplier, 1f * multiplier, 0.2f);
	}

	private void OnPlayerSparkPickedUp()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnSparkPickupSetting>().Value;
		if (multiplier <= 0) return;

		controllers.ActiveController?.Rumble(0f, 0.5f * multiplier, 0.05f);
	}

	private void OnPlayerBoostRingPassed(float boost)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnBoostRingSetting>().Value;
		if (multiplier <= 0) return;

		controllers.ActiveController?.Rumble(0.5f * multiplier, 0.5f * multiplier, 0.2f);
	}

	private void OnPlayerBoardBoosting()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnBoardBoostSetting>().Value;
		if (multiplier <= 0) return;

		controllers.ActiveController?.Rumble(0f, 0.15f * multiplier, 0.1f);
	}

	private void OnPlayerFlyAbilityUsed(bool grounded)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFlySetting>().Value;
		if (multiplier <= 0) return;

		float strength = grounded ? 0.75f : 0.2f;
		controllers.ActiveController?.Rumble(strength * multiplier, strength * multiplier, 0.2f);
	}

	private void OnPlayerGrappleAbilityUsed()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnGrappleSetting>().Value;
		if (multiplier <= 0) return;

		controllers.ActiveController?.Rumble(0.2f * multiplier, 0.2f * multiplier, 0.2f);
	}

	private void OnPlayerGrappleAbilityFinished()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnGrappleSetting>().Value;
		if (multiplier <= 0) return;

		controllers.ActiveController?.Rumble(0.1f * multiplier, 0.1f * multiplier, 0.2f);
	}
}