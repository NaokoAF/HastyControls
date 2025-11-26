using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class PlayerLandPatch
{
	static PlayerLandPatch()
	{
		On.PlayerMovement.Start += (orig, self) =>
		{
			orig(self);
			self.landAction += (character, type, unknown) => Mod.Events.PlayerLanded?.Invoke(character.player, type, unknown);
		};
	}
}
