using HarmonyLib;
using Unity.Mathematics;
using Zorro.Settings;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(HasteSettingsHandler))]
internal static class HasteSettingsPatch
{
	// experimenting with custom settings. need to figure out how to add new localization entries
	//[HarmonyPatch(nameof(HasteSettingsHandler.RegisterPage))]
	//[HarmonyPrefix]
	//static void RegisterPagePrefix(HasteSettingsHandler __instance)
	//{
	//	__instance.AddSetting(new GyroSensitivitySetting());
	//}
}

public class GyroSensitivitySetting : FloatSetting, IExposedSetting
{
	public string GetCategory() => "Controls";
	public UnityEngine.Localization.LocalizedString GetDisplayName() => new("Settings", "GyroSensitivitySetting");

	protected override float GetDefaultValue() => 4f;
	protected override float2 GetMinMaxValue() => new(0f, 10f);

	public override void ApplyValue() { }
}