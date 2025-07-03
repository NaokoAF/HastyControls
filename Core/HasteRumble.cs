using UnityEngine;
using Zorro.Settings;
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

	void Rumble<TSetting>(string id, float lowFrequency, float highFrequency, float duration) where TSetting : FloatSetting, new()
	{
		RumbleEvent rumble;
		if (!rumbleEvents.TryGetValue(id, out rumble))
		{
			rumble = new();
			rumbleEvents[id] = rumble;
		}

		float multiplier = GetSetting<RumbleIntensitySetting>().Value * GetSetting<TSetting>().Value;
		rumble.LowFrequency = lowFrequency * multiplier;
		rumble.HighFrequency = highFrequency * multiplier;
		rumble.Lifetime = duration;
	}

	private void OnPlayerDamaged(Player player, float damage, Transform dealer, EffectSource source)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnDamageSetting>(nameof(OnPlayerDamaged), 1f, 0f, 0.2f);
	}

	private void OnPlayerLanded(Player player, LandingType type, bool arg2)
	{
		if (player != Player.localPlayer) return;

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

		Rumble<RumbleOnLandSetting>(nameof(OnPlayerLanded), low, high, 0.2f);
	}

	private void OnPlayerCharging(Player player, float amount, bool up)
	{
		if (player != Player.localPlayer) return;
		if (!up) return;

		amount = Mathf.Clamp01(amount);
		Rumble<RumbleOnFastRunSetting>(nameof(OnPlayerCharging),
			amount * 0.3f,
			amount * 0.3f,
			0.1f
		);
	}

	private void OnPlayerChargingEnded(Player player)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnFastRunSetting>(nameof(OnPlayerChargingEnded), 1f, 1f, 0.2f);
	}

	private void OnPlayerSparkPickedUp(Player player)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnSparkPickupSetting>(nameof(OnPlayerSparkPickedUp), 0f, 0.2f, 0.1f);
	}

	private void OnPlayerBoostRingPassed(Player player, float boost)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnBoostRingSetting>(nameof(OnPlayerBoostRingPassed), 0.75f, 0.75f, 0.2f);
	}

	private void OnPlayerBoardBoosting(Player player)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnBoardBoostSetting>(nameof(OnPlayerBoardBoosting), 0f, 0.3f, 0.1f);
	}

	private void OnPlayerFlyAbilityUsed(Player player, bool grounded)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnFlySetting>(nameof(OnPlayerFlyAbilityUsed), 0.75f, 0.75f, 0.2f);
	}

	private void OnPlayerGrappleAbilityUsed(Player player)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnGrappleSetting>(nameof(OnPlayerGrappleAbilityUsed), 0.75f, 0.75f, 0.2f);
	}

	private void OnPlayerGrappleAbilityFinished(Player player)
	{
		if (player != Player.localPlayer) return;

		Rumble<RumbleOnGrappleSetting>(nameof(OnPlayerGrappleAbilityFinished), 0.6f, 0.6f, 0.2f);
	}

	class RumbleEvent
	{
		public float LowFrequency;
		public float HighFrequency;
		public float Lifetime;
	}
}