using HarmonyLib;
using Unity.Mathematics;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(GamepadSensitivitySetting))]
internal static class GamepadSensitivitySettingPatch
{
	[HarmonyPatch("GetMinMaxValue")]
	[HarmonyPostfix]
	static void GetMinMaxValuePostfix(ref float2 __result)
	{
		// Increase max controller sensitivity to 10
		__result.y = 10f;
	}
}