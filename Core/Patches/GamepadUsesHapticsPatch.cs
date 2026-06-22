using HastyControls.SDL3;
using MonoMod.RuntimeDetour;
using Zorro.ControllerSupport;

namespace HastyControls.Core.Patches;

// patches the game's controller haptics check to check SDL if all else fails
// this *should* allow dualsense controllers to use the game's "haptics" rumble profile through bluetooth,
// which unity doesn't normally detect
internal class GamepadUsesHapticsPatch : IHastyPatch
{
	private HastyControlsMod? mod;
	private Hook? hook;

	public void Patch(HastyControlsMod mod)
	{
		this.mod = mod;
		hook = new(typeof(InputHandler).GetMethod(nameof(InputHandler.GamepadUsesHaptics))!, GamepadUsesHaptics);
	}

	private delegate bool orig_GamepadUsesHaptics();

	private bool GamepadUsesHaptics(orig_GamepadUsesHaptics orig)
	{
		if (orig()) return true;

		SDL_GamepadType type = mod!.ControllerManager.ActiveController?.GamepadType ?? SDL_GamepadType.SDL_GAMEPAD_TYPE_UNKNOWN;
		return type == SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5 ||
		       type == SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO ||
		       type == SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_LEFT ||
		       type == SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_RIGHT ||
		       type == SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_PAIR ||
		       type == SDL_GamepadType.SDL_GAMEPAD_TYPE_STEAM;
	}
}