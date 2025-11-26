using System.Reflection;
using Landfall.Modding;

namespace HastyControls.Core.Patches;

[LandfallPlugin]
internal static class AbilityGrapplePatch
{
	static FieldInfo playerField = typeof(Grapple).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	static AbilityGrapplePatch()
	{
		On.Grapple.Activate += (orig, self) =>
		{
			Player player = (Player)playerField.GetValue(self);
			Mod.Events.PlayerGrappleAbilityUsed?.Invoke(player);
			
			orig(self);
		};

		On.Grapple.EndGrapple += (orig, self) =>
		{
			Player player = (Player)playerField.GetValue(self);
			Mod.Events.PlayerGrappleAbilityFinished?.Invoke(player);
			
			orig(self);
		};
	}
}
