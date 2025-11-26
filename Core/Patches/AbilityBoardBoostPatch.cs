using System.Reflection;
using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class AbilityBoardBoostPatch
{
	static FieldInfo playerField = typeof(A_BoardBoost).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	static FieldInfo landBoostingField = typeof(A_BoardBoost).GetField("landBoosting", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	static AbilityBoardBoostPatch()
	{
		On.A_BoardBoost.Update += (orig, self) =>
		{
			bool prevBoosting = (bool)landBoostingField.GetValue(self);
			
			orig(self);

			bool boosting = (bool)landBoostingField.GetValue(self);
			PlayerCharacter player = (PlayerCharacter)playerField!.GetValue(self);
			if (boosting != prevBoosting)
				Mod.Events.PlayerBoardBoostChanged?.Invoke(player.player, boosting);

			if (boosting)
				Mod.Events.PlayerBoardBoosting?.Invoke(player.player);
		};
	}
}
