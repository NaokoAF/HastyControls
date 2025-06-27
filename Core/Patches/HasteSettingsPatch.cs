using HarmonyLib;
using HastyControls.Core.Settings;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(GameHandler))]
internal static class HasteSettingsPatch
{
	[HarmonyPatch("Awake")]
	[HarmonyPostfix]
	static void AwakePostfix(GameHandler __instance)
	{
		Mod.Logger.Msg("Initializing HastySettings");
		HastySettings.Initialize(__instance.SettingsHandler);
	}
}
