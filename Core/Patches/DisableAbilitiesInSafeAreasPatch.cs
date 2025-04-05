using HarmonyLib;
using static HastyControls.Core.Settings.HastySettings;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(PlayerCharacter.PlayerInput))]
internal static class DisableAbilitiesInSafeAreasPatch
{
	[HarmonyPatch(nameof(PlayerCharacter.PlayerInput.SampleInput))]
	[HarmonyPostfix]
	static void SampleInputPostfix(PlayerCharacter character, ref bool ___abilityWasPressed, ref bool ___abilityIsPressed)
	{
		if (!GetSetting<GeneralDisableAbilitiesInSafeZonesSetting>().Value)
			return;

		if (GM_Shop.instance != null || GM_Rest.instance != null)
		{
			___abilityWasPressed = false;
			___abilityIsPressed = false;
		}
	}
}