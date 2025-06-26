using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(PlayerSlowMovement))]
internal static class PlayerFastRunPatch
{
	[HarmonyPatch("CheckMode")]
	[HarmonyPrefix]
	static void CheckModePrefix(PlayerSlowMovement __instance, ref float __state, float ___chargeCounter)
	{
		__state = ___chargeCounter;
	}

	[HarmonyPatch("CheckMode")]
	[HarmonyPostfix]
	static void CheckModePostfix(PlayerSlowMovement __instance, ref float __state, float ___chargeCounter, PlayerCharacter ___player)
	{
		float chargeCounter = ___chargeCounter;
		bool charging = chargeCounter > 0f;
		bool wasCharging = __state > 0f;
		if (charging != wasCharging)
		{
			Mod.Events.PlayerChargingChanged?.Invoke(___player.player, charging);
		}

		if (charging)
		{
			bool chargingUp = chargeCounter >= __state;
			Mod.Events.PlayerCharging?.Invoke(___player.player, chargeCounter, chargingUp);
		}
	}

	[HarmonyPatch("GoFast")]
	[HarmonyPostfix]
	static void GoFastPostfix(PlayerSlowMovement __instance, PlayerCharacter ___player)
	{
		Mod.Events.PlayerChargingEnded?.Invoke(___player.player);
	}
}
