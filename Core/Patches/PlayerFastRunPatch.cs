using System.Reflection;
using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class PlayerFastRunPatch
{
	static FieldInfo playerField = typeof(PlayerSlowMovement).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	static FieldInfo chargeCounterField = typeof(PlayerSlowMovement).GetField("chargeCounter", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	static PlayerFastRunPatch()
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
				Mod.Events.PlayerChargingChanged?.Invoke(player.player, charging);
			}

			if (charging)
			{
				bool chargingUp = chargeCounter >= prevChargeCounter;
				Mod.Events.PlayerCharging?.Invoke(player.player, chargeCounter, chargingUp);
			}
		};
		
		On.PlayerSlowMovement.GoFast += (orig, self) =>
		{
			orig(self);
			
			PlayerCharacter player = (PlayerCharacter)playerField.GetValue(self);
			Mod.Events.PlayerChargingEnded?.Invoke(player.player);
		};
	}
}
