using HarmonyLib;
using HastyControls.Core;
using HastyControls.SDL3;
using System.Reflection;
using UnityEngine;

namespace HastyControls.Landfall;

public class LandfallModLifetime : MonoBehaviour
{
	void Awake()
	{
		var logger = new UnityDebugLogger();
		logger.Msg($"Hello from {ModInfo.Name} {ModInfo.Version}");

		// Landfall's modding library loads all DLLs in the workshop folder, but when it tries to load SDL an error happens
		// so we need to change the extension to something else to avoid that error
		// I'm sure there's a better way around this, but this is all I could come up with
		string sdlPath = Path.Combine(GetAssemblyDirectory(), "SDL3.dll.assetbundle");
		logger.Msg($"Loading SDL from {sdlPath}");
		SDL sdl = new(sdlPath);

		// apply all harmony patches
		// NOTE: specifying an assembly on PatchAll seems to cause an error
		logger.Msg($"Applying Harmony patches with ID {ModInfo.Guid}");
		Harmony harmony = new(ModInfo.Guid);
		harmony.PatchAll();

		logger.Msg($"Initializing mod");
		Mod.Logger = logger;
		Mod.Initialize(sdl);
	}

	void Update()
	{
		Mod.Update();
	}

	void OnApplicationQuit()
	{
		Mod.Deinitialize();
	}

	// janky solution to find where the workshop folder is
	static string GetAssemblyDirectory()
	{
		string codeBase = Assembly.GetExecutingAssembly().CodeBase;
		UriBuilder uri = new UriBuilder(codeBase);
		string path = Uri.UnescapeDataString(uri.Path);
		return Path.GetDirectoryName(path);
	}
}