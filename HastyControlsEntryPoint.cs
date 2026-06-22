using System.Reflection;
using System.Runtime.InteropServices;
using HastyControls.Core;
using HastyControls.SDL3;
using Landfall.Modding;
using UnityEngine;
using ILogger = HastyControls.Core.ILogger;

namespace HastyControls;

[LandfallPlugin]
internal static class HastyControlsEntryPoint
{
	static HastyControlsEntryPoint()
	{
		ILogger logger = new UnityDebugLogger();
		logger.Msg($"Hello from {ModInfo.Name} {ModInfo.Version}");

		string sdlPath = GetSdlPath();
		logger.Msg($"Loading SDL from {sdlPath}");

		SDLManager sdl = new(new SDL(sdlPath));
		logger.Msg($"SDL Version: {sdl.Revision}");

		if (!sdl.Init()) throw new Exception($"Failed to initialize SDL: {sdl.CurrentError}");

		// create an object that lives forever so we can listen to Unity events
		GameObject gameObject = new(ModInfo.Name);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);

		HastyControlsMod mod = gameObject.AddComponent<HastyControlsMod>();
		mod.Initialize(logger, sdl);
	}

	// janky solution to find where the workshop folder is
	private static string GetAssemblyDirectory()
	{
		string codeBase = Assembly.GetExecutingAssembly().CodeBase;
		UriBuilder uri = new UriBuilder(codeBase);
		string path = Uri.UnescapeDataString(uri.Path);
		return Path.GetDirectoryName(path)!;
	}

	private static string GetSdlPath()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			// Landfall's mod loader looks for every .dll file recursively and calls Assembly.LoadFrom on them.
			// Haste finds SDL3.dll, tries to load it like a C# assembly, fails, and shows a big warning to the user.
			// The mod and game continue to work, but the warning happens upon every boot, which could be annoying.
			// As a workaround, we simply change the file's extension to something else.
			return Path.Combine(GetAssemblyDirectory(), "native", "win-x64", "SDL3.dll.dontloadme");
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return Path.Combine(GetAssemblyDirectory(), "native", "linux-x64", "libSDL3.so");
		}

		throw new PlatformNotSupportedException();
	}
}