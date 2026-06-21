using System.Reflection;

namespace HastyControls.Core.Patches;

internal class AbilitySlomoPatch : IHastyPatch
{
	static FieldInfo playerField = typeof(Ability_Slomo).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	static FieldInfo currentlyActiveField = typeof(Ability_Slomo).GetField("currentlyActive", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	public void Patch(HastyControlsMod mod)
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
					mod.Events.PlayerSlowMoAbilityUsed?.Invoke(player.player);
				else
					mod.Events.PlayerSlowMoAbilityFinished?.Invoke(player.player);
			}
		};
	}
}
