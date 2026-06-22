using HastyControls.Core.Settings;

namespace HastyControls.Core.Patches;

internal class HasteSettingsPatch : IHastyPatch
{
	public void Patch(HastyControlsMod mod)
	{
		On.GameHandler.Awake += (orig, self) =>
		{
			orig(self);

			mod.Logger.Msg("Initializing HastySettings");
			HastySettings.Initialize(self.SettingsHandler);
		};
	}
}