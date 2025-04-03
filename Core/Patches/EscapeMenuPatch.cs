using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(EscapeMenu))]
internal static class EscapeMenuPatch
{
	[HarmonyPatch(nameof(EscapeMenu.Close))]
	[HarmonyPrefix]
	static void ClosePrefix()
	{
		Mod.Events.EscapeMenuClosed?.Invoke();
	}

	[HarmonyPatch("Open")]
	[HarmonyPrefix]
	static void OpenPrefix()
	{
		Mod.Events.EscapeMenuOpened?.Invoke();
	}
}