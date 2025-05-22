using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(PlayerMovement))]
internal static class PlayerLandPatch
{
	[HarmonyPatch("Start")]
	[HarmonyPostfix]
	static void StartPostfix(PlayerMovement __instance)
	{
		__instance.landAction += (character, type, unknown) => Mod.Events.PlayerLanded?.Invoke(type, unknown);
	}
}
