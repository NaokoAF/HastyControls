using Landfall.Modding;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class DisableAbilitiesInSafeAreasPatch
{
	static DisableAbilitiesInSafeAreasPatch()
	{
		On.PlayerCharacter.PlayerInput.SampleInput += (orig, self, character, autoRun) =>
		{
			orig(self, character, autoRun);
			
			if (!GetSetting<GeneralDisableAbilitiesInSafeZonesSetting>().Value)
				return;

			if (GM_Shop.instance != null || GM_Rest.instance != null)
			{
				self.abilityWasPressed = false;
				self.abilityIsPressed = false;
			}
		};
	}
}