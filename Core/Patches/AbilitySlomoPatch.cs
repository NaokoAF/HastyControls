using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(Ability_Slomo))]
internal static class AbilitySlomoPatch
{
	[HarmonyPatch("Update")]
	[HarmonyPrefix]
	static void UpdatePrefix(Ability_Slomo __instance, ref bool __state, ref bool ___currentlyActive)
	{
		__state = ___currentlyActive;
	}

	[HarmonyPatch("Update")]
	[HarmonyPostfix]
	static void UpdatePostfix(Ability_Slomo __instance, ref bool __state, ref bool ___currentlyActive)
	{
		bool currentlyActive = ___currentlyActive;
		if (currentlyActive != __state)
		{
			if (currentlyActive)
				Mod.Events.PlayerSlowMoAbilityUsed?.Invoke();
			else
				Mod.Events.PlayerSlowMoAbilityFinished?.Invoke();
		}
	}
}
