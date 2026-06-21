using HastyControls.Core;
using Landfall.Modding;
using UnityEngine;

namespace HastyControls;

[LandfallPlugin]
internal static class HastyControlsEntryPoint
{
	static HastyControlsEntryPoint()
	{
		// create an object that lives forever so we can listen to Unity events
		GameObject gameObject = new(ModInfo.Name);
		gameObject.AddComponent<HastyControlsMod>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}
}