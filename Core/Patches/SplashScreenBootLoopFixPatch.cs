using HarmonyLib;
using System.Collections;
using System.Reflection;

namespace HastyControls.Core.Patches;

// the game's splash screen subscribes to InputSystem.onAnyButtonPress to allow skipping it early
// for whatever reason, when this mod is installed, it can cause the splash screen to loop forever
// quick and dirty fix: wait a single frame before skipping
[HarmonyPatch(typeof(TransitionInAndOut))]
internal static class SplashScreenBootLoopFixPatch
{
	static readonly MethodInfo GoToMenu = typeof(TransitionInAndOut).GetMethod("GoToMenu", BindingFlags.NonPublic | BindingFlags.Instance);

	[HarmonyPatch(nameof(TransitionInAndOut.GoOut))]
	[HarmonyPrefix]
	static bool GoOutPrefix(TransitionInAndOut __instance, bool ___skipped)
	{
		if (!___skipped)
		{
			__instance.StartCoroutine(GoOutDelayed(__instance));
		}
		return false;
	}

	static IEnumerator GoOutDelayed(TransitionInAndOut __instance)
	{
		// wait a frame
		yield return null;

		// transition to menu
		UI_TransitionHandler.instance.Transition(() => GoToMenu.Invoke(__instance, []), "Stripes", 0f, 0.5f);
	}
}