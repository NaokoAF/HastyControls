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

		// load config AFTER initializing the mod to avoid errors
		Mod.Initialize(LandfallPluginEntryPoint.sdl);
		LoadConfig();
	}

	void Update()
	{
		Mod.Update();
	}

	// reload config when application is tabbed back into
	// janky and simple, good enough, but could potentially cause issues
	// maybe we should have some debouncing on this just in case?
	void OnApplicationFocus(bool focus)
	{
		if (focus)
			LoadConfig();
	}

	void OnApplicationQuit()
	{
		Mod.Deinitialize();
	}

	void LoadConfig()
	{
		try
		{
			// get path and ensure folder exists
			string path = Path.Combine(Application.dataPath, "../Config/HastyControls.ini");
			Directory.CreateDirectory(Path.GetDirectoryName(path));

			// load and save file
			LandfallConfig config = LandfallConfig.LoadFromFile(path);
			config.SaveToFile(path);

			// load config
			config.UpdateConfig(Mod.Config);
			Mod.UpdateConfig();
			Mod.Logger.Msg($"HastyControls config loaded!");
		}
		catch (Exception e)
		{
			Mod.Logger.Error($"Error loading HastyControls config: {e}");
		}
	}
}