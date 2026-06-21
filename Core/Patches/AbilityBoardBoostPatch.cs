using System.Reflection;

namespace HastyControls.Core.Patches;

internal class AbilityBoardBoostPatch : IHastyPatch
{
	static FieldInfo playerField = typeof(A_BoardBoost).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	static FieldInfo landBoostingField = typeof(A_BoardBoost).GetField("landBoosting", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	public void Patch(HastyControlsMod mod)
	{
		On.A_BoardBoost.Update += (orig, self) =>
		{
			bool prevBoosting = (bool)landBoostingField.GetValue(self);
			
			orig(self);

			bool boosting = (bool)landBoostingField.GetValue(self);
			PlayerCharacter player = (PlayerCharacter)playerField!.GetValue(self);
			if (boosting != prevBoosting)
				mod.Events.PlayerBoardBoostChanged?.Invoke(player.player, boosting);

			if (boosting)
				mod.Events.PlayerBoardBoosting?.Invoke(player.player);
		};
	}
}
