using HarmonyLib;
using HastyControls.SDL3;
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
		if (__result) return;

		SDL_GamepadType type = Mod.ControllerManager?.ActiveController?.GamepadType ?? SDL_GamepadType.SDL_GAMEPAD_TYPE_UNKNOWN;
		__result = (
			type == SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5 ||
			type == SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO
		);
	}
}