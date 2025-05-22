using UnityEngine;

namespace HastyControls.Core;

public class HasteEvents
{
	public Action<float, Transform, EffectSource>? PlayerDamaged;
	public Action<LandingType, bool>? PlayerLanded;
	public Action<bool>? PlayerChargingChanged;
	public Action<float, bool>? PlayerCharging;
	public Action? PlayerChargingEnded;
	public Action<bool>? PlayerBoardBoostChanged;
	public Action? PlayerBoardBoosting;
	public Action<bool>? PlayerFlyAbilityUsed;
	public Action? PlayerGrappleAbilityUsed;
	public Action? PlayerGrappleAbilityFinished;
	public Action? PlayerSlowMoAbilityUsed;
	public Action? PlayerSlowMoAbilityFinished;
	public Action<EffectSource, int>? PlayerResourceReceived;
	public Action? PlayerSparkPickedUp;
	public Action<float>? PlayerHealthChanged;
	public Action<float>? PlayerEnergyChanged;
	public Action<float>? PlayerBoostRingPassed;

	public Action? EscapeMenuOpened;
	public Action? EscapeMenuClosed;
}