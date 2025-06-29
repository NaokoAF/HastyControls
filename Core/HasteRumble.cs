using UnityEngine;
using UnityEngine.InputSystem;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core;

public class HasteRumble
{
	ControllerManager controllerManager;
	Dictionary<string, RumbleEvent> rumbleEvents = new();

	public HasteRumble(HasteEvents events, ControllerManager controllerManager)
	{
		this.controllerManager = controllerManager;

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

		controllerManager.ActiveController?.Rumble(low, high, 0.5f);
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

	private void OnPlayerDamaged(Player player, float damage, Transform dealer, EffectSource source)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnDamageSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerDamaged), 1f * multiplier, 0f, 0.2f);
	}

	private void OnPlayerLanded(Player player, LandingType type, bool arg2)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnLandSetting>().Value;
		if (multiplier <= 0) return;

		float low;
		float high;
		switch (type)
		{
			case LandingType.Bad:
				low = 0.5f;
				high = 0f;
				break;
			case LandingType.Ok:
				low = 0f;
				high = 1f;
				break;
			case LandingType.Good:
				low = 0f;
				high = 1f;
				break;
			case LandingType.Perfect:
				low = 0.2f;
				high = 0.8f;
				break;
			default:
				return;
		}

		Rumble(nameof(OnPlayerLanded), low * multiplier, high * multiplier, 0.2f);
	}

	private void OnPlayerCharging(Player player, float amount, bool up)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFastRunSetting>().Value;
		if (multiplier <= 0) return;

		if (up)
		{
			amount = Mathf.Clamp01(amount);
			Rumble(nameof(OnPlayerCharging),
				amount * 0.3f * multiplier,
				amount * 0.3f * multiplier,
				0.1f
			);
		}
	}

	private void OnPlayerChargingEnded(Player player)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFastRunSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerChargingEnded), 1f * multiplier, 1f * multiplier, 0.2f);
	}

	private void OnPlayerSparkPickedUp(Player player)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnSparkPickupSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerSparkPickedUp), 0f, 0.2f * multiplier, 0.1f);
	}

	private void OnPlayerBoostRingPassed(Player player, float boost)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnBoostRingSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerBoostRingPassed), 0.75f * multiplier, 0.75f * multiplier, 0.2f);
	}

	private void OnPlayerBoardBoosting(Player player)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnBoardBoostSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerBoardBoosting), 0f, 0.3f * multiplier, 0.1f);
	}

	private void OnPlayerFlyAbilityUsed(Player player, bool grounded)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnFlySetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerFlyAbilityUsed), 0.75f * multiplier, 0.75f * multiplier, 0.2f);
	}

	private void OnPlayerGrappleAbilityUsed(Player player)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnGrappleSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerGrappleAbilityUsed), 0.75f * multiplier, 0.75f * multiplier, 0.2f);
	}

	private void OnPlayerGrappleAbilityFinished(Player player)
	{
		if (player != Player.localPlayer) return;

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<RumbleOnGrappleSetting>().Value;
		if (multiplier <= 0) return;

		Rumble(nameof(OnPlayerGrappleAbilityFinished), 0.6f * multiplier, 0.6f * multiplier, 0.2f);
	}

	class RumbleEvent
	{
		public float LowFrequency;
		public float HighFrequency;
		public float Lifetime;
	}
}