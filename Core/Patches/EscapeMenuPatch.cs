namespace HastyControls.Core.Patches;

internal class EscapeMenuPatch : IHastyPatch
{
	public void Patch(HastyControlsMod mod)
	{
		On.EscapeMenu.Close += (orig, self) =>
		{
			orig(self);
			mod.Events.EscapeMenuToggled?.Invoke(false);
		};

		On.EscapeMenu.Open += (orig, self, disconnected) =>
		{
			orig(self, disconnected);
			mod.Events.EscapeMenuToggled?.Invoke(true);
		};
	}
}