using UnityEngine;
using UnityEngine.InputSystem;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

public class HasteRumble
{
	Dictionary<string, RumbleEvent> rumbleEvents = new();

	public HasteRumble(HasteEvents events)
	{
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

	public void Update(float deltaTime)
	{
		float low = 0f;
		float high = 0f;

		// add up all rumble events and reduce their lifetimes
		foreach (var (id, rumble) in rumbleEvents)
		{
			if (rumble.Lifetime <= 0) continue;

			low += rumble.LowFrequency;
			high += rumble.HighFrequency;
			rumble.Lifetime -= deltaTime;
		}

		// assumes unity will clamp the values for us
		Gamepad.current?.SetMotorSpeeds(low, high);
	}

	void Rumble(string id, float lowFrequency, float highFrequency, float duration)
	{
		RumbleEvent rumble;
		if (!rumbleEvents.TryGetValue(id, out rumble))
		{
			rumble = new();
			rumbleEvents[id] = rumble;
		}

		rumble.LowFrequency = lowFrequency;
		rumble.HighFrequency = highFrequency;
		rumble.Lifetime = duration;
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
			Rumble(nameof(OnPlayerDamaged), damageRatio * multiplier, 0f, 0.2f);
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

		Rumble(nameof(OnPlayerLanded), low * multiplier, high * multiplier, duration);
	}

	private void OnPlayerCharging(float amount, bool up)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFastRunSetting>().Value;
		if (multiplier <= 0) return;

		if (up)
		{
			amount = Mathf.Clamp01(amount);
			Rumble(nameof(OnPlayerCharging),
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

		Rumble(nameof(OnPlayerChargingEnded), 1f * multiplier, 1f * multiplier, 0.2f);
	}

	private void OnPlayerSparkPickedUp()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnSparkPickupSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerSparkPickedUp), 0f, 0.5f * multiplier, 0.05f);
	}

	private void OnPlayerBoostRingPassed(float boost)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnBoostRingSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerBoostRingPassed), 0.5f * multiplier, 0.5f * multiplier, 0.2f);
	}

	private void OnPlayerBoardBoosting()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnBoardBoostSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerBoardBoosting), 0f, 0.15f * multiplier, 0.1f);
	}

	private void OnPlayerFlyAbilityUsed(bool grounded)
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFlySetting>().Value;
		if (multiplier <= 0) return;

		float strength = grounded ? 0.75f : 0.2f;
		Rumble(nameof(OnPlayerFlyAbilityUsed), strength * multiplier, strength * multiplier, 0.2f);
	}

	private void OnPlayerGrappleAbilityUsed()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnGrappleSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerGrappleAbilityUsed), 0.2f * multiplier, 0.2f * multiplier, 0.2f);
	}

	private void OnPlayerGrappleAbilityFinished()
	{
		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnGrappleSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerGrappleAbilityFinished), 0.1f * multiplier, 0.1f * multiplier, 0.2f);
	}

	class RumbleEvent
	{
		public float LowFrequency;
		public float HighFrequency;
		public float Lifetime;
	}
}