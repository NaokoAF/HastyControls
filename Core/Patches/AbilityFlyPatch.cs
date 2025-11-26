using System.Reflection;
using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class AbilityFlyPatch
{
	static FieldInfo playerField = typeof(Ability_Fly).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	static AbilityFlyPatch()
	{
		On.Ability_Fly.Update += (orig, self) =>
		{
			PlayerCharacter player = (PlayerCharacter)playerField.GetValue(self);
			bool ground = player.data.mostlyGrounded;
			float cost = ground ? self.groundCost : self.airCost;
			if (player.input.abilityWasPressed && player.player.data.energy >= cost)
			{
				Mod.Events.PlayerFlyAbilityUsed?.Invoke(player.player, ground);
			}
			
			orig(self);
		};
	}
}
