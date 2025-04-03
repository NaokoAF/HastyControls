using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(PlayerSlowMovement))]
internal static class PlayerFastRunPatch
{
	[HarmonyPatch("CheckMode")]
	[HarmonyPrefix]
	static void CheckModePrefix(PlayerSlowMovement __instance, ref float __state, ref float ___chargeCounter)
	{
		__state = ___chargeCounter;
	}

	[HarmonyPatch("CheckMode")]
	[HarmonyPostfix]
	static void CheckModePostfix(PlayerSlowMovement __instance, ref float __state, ref float ___chargeCounter)
	{
		float chargeCounter = ___chargeCounter;
		bool charging = chargeCounter > 0f;
		bool wasCharging = __state > 0f;
		if (charging != wasCharging)
		{
			Mod.Events.PlayerChargingChanged?.Invoke(charging);
		}

		if (charging)
		{
			bool chargingUp = chargeCounter >= __state;
			Mod.Events.PlayerCharging?.Invoke(chargeCounter, chargingUp);
		}
	}

	[HarmonyPatch("GoFast")]
	[HarmonyPostfix]
	static void GoFastPostfix(PlayerSlowMovement __instance)
	{
		Mod.Events.PlayerChargingEnded?.Invoke();
	}
}
