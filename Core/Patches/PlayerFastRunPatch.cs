using System.Reflection;

namespace HastyControls.Core.Patches;

internal class PlayerFastRunPatch : IHastyPatch
{
	private static readonly FieldInfo playerField =
		typeof(PlayerSlowMovement).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;

	private static readonly FieldInfo chargeCounterField =
		typeof(PlayerSlowMovement).GetField("chargeCounter", BindingFlags.Instance | BindingFlags.NonPublic)!;

	public void Patch(HastyControlsMod mod)
	{
		On.PlayerSlowMovement.CheckMode += (orig, self) =>
		{
			float prevChargeCounter = (float)chargeCounterField.GetValue(self);

			orig(self);

			float chargeCounter = (float)chargeCounterField.GetValue(self);
			PlayerCharacter player = (PlayerCharacter)playerField.GetValue(self);

			bool charging = chargeCounter > 0f;
			bool wasCharging = prevChargeCounter > 0f;
			if (charging != wasCharging)
			{
				mod.Events.PlayerChargingChanged?.Invoke(player.player, charging);
			}

			if (charging)
			{
				bool chargingUp = chargeCounter >= prevChargeCounter;
				mod.Events.PlayerCharging?.Invoke(player.player, chargeCounter, chargingUp);
			}
		};

		On.PlayerSlowMovement.GoFast += (orig, self) =>
		{
			orig(self);

			PlayerCharacter player = (PlayerCharacter)playerField.GetValue(self);
			mod.Events.PlayerChargingEnded?.Invoke(player.player);
		};
	}
}