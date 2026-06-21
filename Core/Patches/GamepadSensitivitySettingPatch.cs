using Unity.Mathematics;

namespace HastyControls.Core.Patches;

internal class GamepadSensitivitySettingPatch : IHastyPatch
{
	public void Patch(HastyControlsMod mod)
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