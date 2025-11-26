using System.Collections;
using System.Reflection;
using Landfall.Modding;

namespace HastyControls.Core.Patches;

// the game's splash screen subscribes to InputSystem.onAnyButtonPress to allow skipping it early
// for whatever reason, when this mod is installed, it can cause the splash screen to loop forever
// quick and dirty fix: wait a single frame before skipping
[LandfallPlugin]
internal static class SplashScreenBootLoopFixPatch
{
	static FieldInfo skippedField = typeof(TransitionInAndOut).GetField("skipped", BindingFlags.Instance | BindingFlags.NonPublic)!;
	static readonly MethodInfo GoToMenuMethod = typeof(TransitionInAndOut).GetMethod("GoToMenu", BindingFlags.Instance | BindingFlags.NonPublic)!;

	static SplashScreenBootLoopFixPatch()
	{
		On.TransitionInAndOut.GoOut += (orig, self) =>
		{
			if (!((bool)skippedField.GetValue(self)))
			{
				self.StartCoroutine(GoOutDelayed(self));
			}
			
			orig(self);
		};
	}
	
	static IEnumerator GoOutDelayed(TransitionInAndOut self)
	{
		// wait a frame
		yield return null;

		// transition to menu
		UI_TransitionHandler.instance.Transition(() => GoToMenuMethod.Invoke(self, []), "Stripes", 0f, 0.5f);
	}
}