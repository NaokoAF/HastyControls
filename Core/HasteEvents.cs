using UnityEngine;

namespace HastyControls.Core;

public class HasteEvents
{
	public Action<Player, float, Transform, EffectSource>? PlayerDamaged;
	public Action<Player, LandingType, bool>? PlayerLanded;
	public Action<Player, bool>? PlayerChargingChanged;
	public Action<Player, float, bool>? PlayerCharging;
	public Action<Player>? PlayerChargingEnded;
	public Action<Player, bool>? PlayerBoardBoostChanged;
	public Action<Player>? PlayerBoardBoosting;
	public Action<Player, bool>? PlayerFlyAbilityUsed;
	public Action<Player>? PlayerGrappleAbilityUsed;
	public Action<Player>? PlayerGrappleAbilityFinished;
	public Action<Player>? PlayerSlowMoAbilityUsed;
	public Action<Player>? PlayerSlowMoAbilityFinished;
	public Action<Player, EffectSource, int>? PlayerResourceReceived;
	public Action<Player>? PlayerSparkPickedUp;
	public Action<Player, float>? PlayerHealthChanged;
	public Action<Player, float>? PlayerEnergyChanged;
	public Action<Player, float>? PlayerBoostRingPassed;

	public Action? EscapeMenuOpened;
	public Action? EscapeMenuClosed;
}