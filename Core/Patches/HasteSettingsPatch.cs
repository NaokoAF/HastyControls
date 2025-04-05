using HarmonyLib;
using HastyControls.Core.Settings;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(HasteSettingsHandler))]
internal static class HasteSettingsPatch
{
	[HarmonyPatch(nameof(HasteSettingsHandler.RegisterPage))]
	[HarmonyPrefix]
	static void RegisterPagePrefix(HasteSettingsHandler __instance)
	{
		HastySettings.Initialize();
	}
}
