using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(Grapple))]
internal static class AbilityGrapplePatch
{
	[HarmonyPatch(nameof(Grapple.Activate))]
	[HarmonyPrefix]
	static void ActivatePrefix()
	{
		Mod.Events.PlayerGrappleAbilityUsed?.Invoke();
	}

	[HarmonyPatch("EndGrapple")]
	[HarmonyPrefix]
	static void EndGrapplePrefix()
	{
		Mod.Events.PlayerGrappleAbilityFinished?.Invoke();
	}
}
