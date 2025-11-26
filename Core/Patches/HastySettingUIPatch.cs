using System.Reflection;
using HastyControls.Core.Settings;
using System.Runtime.CompilerServices;
using Landfall.Modding;
using MonoMod.RuntimeDetour;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Settings;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class HastySettingUIPatch
{
	static List<Hook> hooks = new();
	static FieldInfo m_canvasGroupField = typeof(SettingsUICell).GetField("m_canvasGroup", BindingFlags.Instance | BindingFlags.NonPublic)!;

	static ConditionalWeakTable<SettingsUICell, CellValues> settingsMap = new();
	
	class CellValues(IHastySetting setting, LayoutElement layoutElement, CanvasGroup canvasGroup)
	{
		public IHastySetting Setting = setting;
		public LayoutElement LayoutElement = layoutElement;
		public CanvasGroup CanvasGroup = canvasGroup;
	}
	
	static HastySettingUIPatch()
	{
		hooks.Add(new(typeof(SettingsUICell).GetMethod("Setup", BindingFlags.Instance | BindingFlags.Public)!, Setup));
		hooks.Add(new(typeof(SettingsUICell).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic)!, Update));
	}

	delegate void orig_Setup(SettingsUICell self, Setting setting);
	delegate void orig_Update(SettingsUICell self);
	
	static void Setup(orig_Setup orig, SettingsUICell self, Setting setting)
	{
		orig(self, setting);
		
		// SettingsUICell doesn't store the setting within itself, so we need to store it ourselves
		// ConditionalWeakTable is a weak dictionary, so it's perfect for this situation
		if (setting is IHastySetting hastySetting)
		{
			// also add a LayoutElement so we can hide the cell
			LayoutElement layoutElement = self.gameObject.AddComponent<LayoutElement>();
			CanvasGroup canvasGroup = (CanvasGroup)m_canvasGroupField.GetValue(self);
			
			settingsMap.Add(self, new(hastySetting, layoutElement, canvasGroup));
		}
	}
	
	static void Update(orig_Update orig, SettingsUICell self)
	{
		orig(self);
		
		if (settingsMap.TryGetValue(self, out var values))
		{
			bool show = values.Setting.ShowCondition?.Invoke() ?? true;

			// hacky solution to hide a UI element without disabling the GameObject
			values.LayoutElement.ignoreLayout = !show; // skip on layout step
			values.CanvasGroup.blocksRaycasts = show; // ignore clicks when hidden
			values.CanvasGroup.alpha = show ? 1 : 0; // hide visually. has the side effect of skipping the fade in animation, which is good
		}
	}
}
