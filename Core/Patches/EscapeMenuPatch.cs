using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class EscapeMenuPatch
{
	static EscapeMenuPatch()
	{
		On.EscapeMenu.Close += (orig, self) =>
		{
			orig(self);
			Mod.Events.EscapeMenuToggled?.Invoke(false);
		};

		On.EscapeMenu.Open += (orig, self, disconnected) =>
		{
			orig(self, disconnected);
			Mod.Events.EscapeMenuToggled?.Invoke(true);
		};
	}
}