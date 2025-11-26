using System.Reflection;
using HastyControls.Core.Settings;
using Landfall.Modding;
using MonoMod.RuntimeDetour;
using Zorro.Settings;
using Zorro.Settings.UI;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class CollapsibleSettingUIPatch
{
	static Hook hook;
	
	static CollapsibleSettingUIPatch()
	{
		hook = new(typeof(ButtonSettingUI).GetMethod("Setup", BindingFlags.Instance | BindingFlags.Public)!, Setup);
	}
	
	delegate void orig_Setup(ButtonSettingUI self, Setting setting, ISettingHandler settingHandler);

	static void Setup(orig_Setup orig, ButtonSettingUI self, Setting setting, ISettingHandler settingHandler)
	{
		orig(self, setting, settingHandler);
		
		if (setting is HastyCollapsibleSetting collapsible)
		{
			self.Label.text = GetString(collapsible);
			collapsible.Clicked += (collapsed) =>
			{
				self.Label.text = GetString(collapsible);
			};
		}
	}
	
	static string GetString(HastyCollapsibleSetting setting) => setting.Collapsed ? $"► Expand" : $"▼ Collapse";
}
