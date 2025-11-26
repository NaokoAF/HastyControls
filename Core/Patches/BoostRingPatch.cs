using System.Reflection;
using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class BoostRingPatch
{
	static FieldInfo usedField = typeof(TriggerEffect).GetField("used", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	static BoostRingPatch()
	{
		On.TriggerEffect.OnTriggerStay += (orig, self, other) =>
		{
			bool prevUsed = (bool)usedField.GetValue(self);
			
			orig(self, other);
			
			bool used = (bool)usedField.GetValue(self);
			if (used && !prevUsed)
			{
				PlayerCharacter player = other.GetComponentInParent<PlayerCharacter>();
				AddVariable_Boost? boostEffect = FindOfType<AddVariable_Boost>(self.effects);
				if (player != null && boostEffect != null)
				{
					Mod.Events.PlayerBoostRingPassed?.Invoke(player.player, boostEffect.addSpeed);
				}
			}
		};
	}

	static T? FindOfType<T>(List<ItemEffect> effects)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			if (effects[i] is T type)
				return type;
		}
		return default;
	}
}
