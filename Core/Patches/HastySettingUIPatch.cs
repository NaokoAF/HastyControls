using HarmonyLib;
using HastyControls.Core.Settings;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(SettingsUICell))]
internal static class HastySettingUIPatch
{
	static AccessTools.FieldRef<SettingsUICell, CanvasGroup> canvasGroupRef = AccessTools.FieldRefAccess<SettingsUICell, CanvasGroup>("m_canvasGroup");

	static ConditionalWeakTable<SettingsUICell, IHastySetting> settingsMap = new();
	static ConditionalWeakTable<SettingsUICell, LayoutElement> layoutElementMap = new();

	[HarmonyPatch(nameof(SettingsUICell.Setup))]
	[HarmonyPostfix]
	static void SetupPostfix(SettingsUICell __instance, Setting setting)
	{
		// SettingsUICell doesn't store the setting within itself, so we need to store it ourselves
		// ConditionalWeakTable is a weak dictionary, so it's perfect for this situtation
		if (setting is IHastySetting hastySetting)
		{
			settingsMap.Add(__instance, hastySetting);

			// also add a LayoutElement so we can hide the cell
			var layoutElement = __instance.gameObject.AddComponent<LayoutElement>();
			layoutElementMap.Add(__instance, layoutElement);
		}
	}

	[HarmonyPatch("Update")]
	[HarmonyPostfix]
	static void UpdatePostfix(SettingsUICell __instance)
	{
		if (settingsMap.TryGetValue(__instance, out var setting) && layoutElementMap.TryGetValue(__instance, out var layoutElement))
		{
			var canvasGroup = canvasGroupRef(__instance);
			bool show = setting.ShowCondition?.Invoke() ?? true;

			// hacky solution to hide a UI element without disabling the GameObject
			layoutElement.ignoreLayout = !show; // skip on layout step
			canvasGroup.blocksRaycasts = show; // ignore clicks when hidden
			canvasGroup.alpha = show ? 1 : 0; // hide visually. has the side effect of skipping the fade in animation, which is good
		}
	}
}
