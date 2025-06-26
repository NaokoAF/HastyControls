using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(Grapple))]
internal static class AbilityGrapplePatch
{
	[HarmonyPatch(nameof(Grapple.Activate))]
	[HarmonyPrefix]
	static void ActivatePrefix(Player ___player)
	{
		Mod.Events.PlayerGrappleAbilityUsed?.Invoke(___player);
	}

	[HarmonyPatch("EndGrapple")]
	[HarmonyPrefix]
	static void EndGrapplePrefix(Player ___player)
	{
		Mod.Events.PlayerGrappleAbilityFinished?.Invoke(___player);
	}
}
