using HastyControls.Core;
using HastyControls.SDL3;
using Landfall.Modding;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;

namespace HastyControls.Landfall;

[LandfallPlugin]
[SuppressMessage("Roslynator", "RCS1102:Make class static", Justification = "LandfallPlugin requires this class to be instanced")]
public unsafe class LandfallPluginEntryPoint
{
	internal static SDL sdl;

	static LandfallPluginEntryPoint()
	{
		Debug.Log($"Hello from {ModInfo.Name} {ModInfo.Version}");

		// Landfall's modding library loads all DLLs in the workshop folder, but when it tries to load SDL an error happens
		// so we need to change the extension to something else to avoid that error
		// I'm sure there's a better way around this, but this is all I could come up with
		sdl = new(Path.Combine(GetAssemblyDirectory(), "SDL3.dll.assetbundle"));

		// create an object that lives forever so we can listen to Unity events
		GameObject go = new(ModInfo.Name);
		go.AddComponent<LandfallModLifetime>();
		UnityEngine.Object.DontDestroyOnLoad(go);
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
