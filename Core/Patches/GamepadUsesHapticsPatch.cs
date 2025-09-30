using HarmonyLib;
using Zorro.ControllerSupport;

namespace HastyControls.Core.Patches;

// patches the game's controller haptics check to check SDL if all else fails
// this *should* allow dualsense controllers to use the game's "haptics" rumble profile through bluetooth,
// which unity doesn't normally detect
[HarmonyPatch(typeof(InputHandler))]
internal static class GamepadUsesHapticsPatch
{
	[HarmonyPatch(nameof(InputHandler.GamepadUsesHaptics))]
	[HarmonyPrefix]
	static void GamepadUsesHapticsPostfix(ref bool __result)
	{
		if (!__result)
		{
			bool isDualsense = Mod.ControllerManager?.ActiveController?.GamepadType == SDL3.SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5;
			__result = isDualsense;
		}
	}
}