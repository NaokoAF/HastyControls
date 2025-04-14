using HarmonyLib;
using HastyControls.Core.Settings;
using Zorro.Settings;
using Zorro.Settings.UI;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(ButtonSettingUI))]
internal static class CollapsibleSettingUIPatch
{
	[HarmonyPatch(nameof(ButtonSettingUI.Setup))]
	[HarmonyPostfix]
	static void SetupPostfix(ButtonSettingUI __instance, Setting setting)
	{
		if (setting is HastyCollapsibleSetting collapsible)
		{
			__instance.Label.text = GetString(collapsible);
			collapsible.Clicked += (collapsed) =>
			{
				__instance.Label.text = GetString(collapsible);
			};
		}
	}

	static string GetString(HastyCollapsibleSetting setting) => setting.Collapsed ? $"► Expand" : $"▼ Collapse";
}
