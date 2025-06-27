using HastyControls.Core;
using Landfall.Modding;
using UnityEngine;

namespace HastyControls.Landfall;

[LandfallPlugin]
public class LandfallPluginEntryPoint
{
	static LandfallPluginEntryPoint()
	{
		// create an object that lives forever so we can listen to Unity events
		GameObject go = new(ModInfo.Name);
		go.AddComponent<LandfallModLifetime>();
		UnityEngine.Object.DontDestroyOnLoad(go);
	}
}
