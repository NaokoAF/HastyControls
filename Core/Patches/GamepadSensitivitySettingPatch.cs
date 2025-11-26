using Landfall.Modding;
using Unity.Mathematics;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class GamepadSensitivitySettingPatch
{
	static GamepadSensitivitySettingPatch()
	{
		On.GamepadSensitivitySetting.GetMinMaxValue += (orig, self) =>
		{
			// Increase max controller sensitivity to 10
			float2 result = orig(self);
			result.y = 10f;
			return result;
		};
	}
}