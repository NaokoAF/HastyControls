using HastyControls.Core.Settings;
using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class HasteSettingsPatch
{
	static HasteSettingsPatch()
	{
		On.GameHandler.Awake += (orig, self) =>
		{
			orig(self);
			
			Mod.Logger.Msg("Initializing HastySettings");
			HastySettings.Initialize(self.SettingsHandler);
		};
	}
}
