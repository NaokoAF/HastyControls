using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(A_BoardBoost))]
internal static class AbilityBoardBoostPatch
{
	[HarmonyPatch("Update")]
	[HarmonyPrefix]
	static void UpdatePrefix(A_BoardBoost __instance, ref bool __state, bool ___landBoosting)
	{
		__state = ___landBoosting;
	}

	[HarmonyPatch("Update")]
	[HarmonyPostfix]
	static void UpdatePostfix(A_BoardBoost __instance, ref bool __state, bool ___landBoosting, PlayerCharacter ___player)
	{
		if (___landBoosting != __state)
		{
			Mod.Events.PlayerBoardBoostChanged?.Invoke(___player.player, ___landBoosting);
		}

		if (___landBoosting)
		{
			Mod.Events.PlayerBoardBoosting?.Invoke(___player.player);
		}
	}
}
