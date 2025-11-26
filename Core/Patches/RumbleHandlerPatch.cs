using System.Reflection;
using Landfall.Modding;
using MonoMod.RuntimeDetour;
using Zorro.ControllerSupport.Rumble;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

// replaces the way the game does rumble to go through SDL instead
// this should allow for better controller support than what Unity normally allows for
[LandfallPlugin]
internal static class RumbleHandlerPatch
{
	static Hook hook;
	static FieldInfo m_activeRumbleInstancesField = typeof(RumbleHandler).GetField("m_activeRumbleInstances", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	static RumbleHandlerPatch()
	{
		hook = new(typeof(RumbleHandler).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic)!, LateUpdate);
	}

	delegate void orig_LateUpdate(RumbleHandler self);
	
	static void LateUpdate(orig_LateUpdate orig, RumbleHandler self)
	{
		if (!self.RumbleEnabled) return;

		// skip if legacy rumble is enabled
		if (GetSetting<RumbleEnabledLegacySetting>().Value) return;

		// combine rumble values from all instances. sorted by priority
		List<RumbleInstance> activeRumbleInstances = (List<RumbleInstance>)m_activeRumbleInstancesField.GetValue(self);
		IOrderedEnumerable<RumbleInstance> sortedInstances = activeRumbleInstances.OrderBy(instance => instance.Priority());
		float low = 0f;
		float high = 0f;
		foreach (RumbleInstance instance in sortedInstances)
		{
			(low, high) = instance.Rumble(low, high);
		}

		// apply rumble
		Mod.ControllerManager?.ActiveController?.Rumble(low * self.RumbleStrength, high * self.RumbleStrength, 0.5f);
	}
}
