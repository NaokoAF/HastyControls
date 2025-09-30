using HarmonyLib;
using HastyControls.Core.Settings;
using Zorro.ControllerSupport.Rumble;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

// replaces the way the game does rumble to go through SDL instead
// this should allow for better controller support than what Unity normally allows for
[HarmonyPatch(typeof(RumbleHandler))]
internal static class RumbleHandlerPatch
{
	[HarmonyPatch("LateUpdate")]
	[HarmonyPrefix]
	static bool LateUpdatePrefix(RumbleHandler __instance, List<RumbleInstance> ___m_activeRumbleInstances)
	{
		if (!__instance.RumbleEnabled) return false;

		// combine rumble values from all instances. sorted by priority
		IOrderedEnumerable<RumbleInstance> sortedInstances = ___m_activeRumbleInstances.OrderBy((RumbleInstance instance) => instance.Priority());
		float low = 0f;
		float high = 0f;
		foreach (RumbleInstance instance in sortedInstances)
		{
			(low, high) = instance.Rumble(low, high);
		}

		// apply rumble
		Mod.ControllerManager?.ActiveController?.Rumble(low * __instance.RumbleStrength, high * __instance.RumbleStrength, 0.5f);
		return false;
	}
}
