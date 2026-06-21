using Zorro.Core.CLI;

namespace HastyControls.Core.Patches;

[ConsoleClassCustomizer(ModInfo.Name)]
internal class ConsoleCommandsPatch : IHastyPatch
{
	private static HastyControlsMod? mod;

	public void Patch(HastyControlsMod mod)
	{
		ConsoleCommandsPatch.mod = mod;
	}

	[ConsoleCommand]
	public static void Version()
	{
		mod!.Logger.Msg($"{ModInfo.Name} {ModInfo.Version} ({mod.SDL.Revision})");
	}

	[ConsoleCommand]
	public static void ListControllers()
	{
		mod!.Logger.Msg($"Controllers: {mod!.SDL.Controllers.Count}");
		foreach (SDLController controller in mod.SDL.Controllers)
		{
			mod.Logger.Msg(mod.GetControllerPrintName(controller));
		}
	}
}