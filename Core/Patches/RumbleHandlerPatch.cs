using System.Reflection;
using MonoMod.RuntimeDetour;
using Zorro.ControllerSupport.Rumble;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

// replaces the way the game does rumble to go through SDL instead
// this should allow for better controller support than what Unity normally allows for
internal class RumbleHandlerPatch : IHastyPatch
{
	private HastyControlsMod? mod;
	private Hook? hook;

	private static readonly FieldInfo m_activeRumbleInstancesField =
		typeof(RumbleHandler).GetField("m_activeRumbleInstances", BindingFlags.Instance | BindingFlags.NonPublic)!;

	public void Patch(HastyControlsMod mod)
	{
		this.mod = mod;
		hook = new(typeof(RumbleHandler).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic)!, LateUpdate);
	}

	private delegate void orig_LateUpdate(RumbleHandler self);

	private void LateUpdate(orig_LateUpdate orig, RumbleHandler self)
	{
		if (!self.RumbleEnabled) return;

		// skip if legacy rumble is enabled
		if (GetSetting<RumbleEnabledLegacySetting>().Value) return;

		// combine rumble values from all instances. sorted by priority
		var activeRumbleInstances = (List<RumbleInstance>)m_activeRumbleInstancesField.GetValue(self);
		var sortedInstances = activeRumbleInstances.OrderBy(instance => instance.Priority());
		
		float low = 0f;
		float high = 0f;
		foreach (RumbleInstance instance in sortedInstances)
		{
			(low, high) = instance.Rumble(low, high);
		}

		// apply rumble
		mod!.ControllerManager.ActiveController?.Rumble(low * self.RumbleStrength, high * self.RumbleStrength, 0.5f);
	}
}