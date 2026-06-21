namespace HastyControls.Core.Patches;

internal class PlayerLandPatch : IHastyPatch
{
	public void Patch(HastyControlsMod mod)
	{
		On.PlayerMovement.Start += (orig, self) =>
		{
			orig(self);
			self.landAction += (character, type, unknown) => mod.Events.PlayerLanded?.Invoke(character.player, type, unknown);
		};
	}
}