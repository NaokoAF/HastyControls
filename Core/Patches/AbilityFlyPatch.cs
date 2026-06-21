using System.Reflection;

namespace HastyControls.Core.Patches;

internal class AbilityFlyPatch : IHastyPatch
{
	static FieldInfo playerField = typeof(Ability_Fly).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	public void Patch(HastyControlsMod mod)
	{
		On.Ability_Fly.Update += (orig, self) =>
		{
			PlayerCharacter player = (PlayerCharacter)playerField.GetValue(self);
			bool ground = player.data.mostlyGrounded;
			float cost = ground ? self.groundCost : self.airCost;
			if (player.input.abilityWasPressed && player.player.data.energy >= cost)
			{
				mod.Events.PlayerFlyAbilityUsed?.Invoke(player.player, ground);
			}
			
			orig(self);
		};
	}
}
