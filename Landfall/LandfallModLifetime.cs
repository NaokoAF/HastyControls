using HarmonyLib;
using HastyControls.Core;
using UnityEngine;

namespace HastyControls.Landfall;

public class LandfallModLifetime : MonoBehaviour
{
	void Awake()
	{
		// apply all harmony patches
		// NOTE: specifying an assembly on PatchAll seems to cause an error
		Harmony harmony = new(ModInfo.Guid);
		harmony.PatchAll();

		Mod.Initialize(LandfallPluginEntryPoint.sdl);
	}

	void Update()
	{
		Mod.Update();
	}

	void OnApplicationQuit()
	{
		Mod.Deinitialize();
	}
}