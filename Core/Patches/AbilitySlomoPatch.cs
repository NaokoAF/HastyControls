using System.Reflection;
using Landfall.Modding;
using MonoMod.RuntimeDetour;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class AbilitySlomoPatch
{
	static FieldInfo playerField = typeof(Ability_Slomo).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	static FieldInfo currentlyActiveField = typeof(Ability_Slomo).GetField("currentlyActive", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	static AbilitySlomoPatch()
	{
		On.Ability_Slomo.Update += (orig, self) =>
		{
			bool prevActive = (bool)currentlyActiveField.GetValue(self);
			
			orig(self);
			
			bool active = (bool)currentlyActiveField.GetValue(self);
			if (active != prevActive)
			{
				PlayerCharacter player = (PlayerCharacter)playerField.GetValue(self);
				if (active)
					Mod.Events.PlayerSlowMoAbilityUsed?.Invoke(player.player);
				else
					Mod.Events.PlayerSlowMoAbilityFinished?.Invoke(player.player);
			}
		};
	}
}
