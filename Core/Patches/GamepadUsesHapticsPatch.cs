using System.Reflection;
using HastyControls.SDL3;
using Landfall.Modding;
using MonoMod.RuntimeDetour;
using Zorro.ControllerSupport;

namespace HastyControls.Core.Patches;

// patches the game's controller haptics check to check SDL if all else fails
// this *should* allow dualsense controllers to use the game's "haptics" rumble profile through bluetooth,
// which unity doesn't normally detect
[LandfallPlugin]
internal static class GamepadUsesHapticsPatch
{
	static Hook hook;
	
	static GamepadUsesHapticsPatch()
	{
		hook = new(typeof(InputHandler).GetMethod("GamepadUsesHaptics")!, GamepadUsesHaptics);
	}

	delegate bool orig_GamepadUsesHaptics();

	static bool GamepadUsesHaptics(orig_GamepadUsesHaptics orig)
	{
		if (orig()) return true;

		SDL_GamepadType type = Mod.ControllerManager?.ActiveController?.GamepadType ?? SDL_GamepadType.SDL_GAMEPAD_TYPE_UNKNOWN;
		return (
			type == SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5 ||
			type == SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO
		);
	}
}