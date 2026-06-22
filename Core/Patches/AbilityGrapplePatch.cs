using System.Reflection;

namespace HastyControls.Core.Patches;

internal class AbilityGrapplePatch : IHastyPatch
{
	private static readonly FieldInfo playerField =
		typeof(Grapple).GetField("player", BindingFlags.Instance | BindingFlags.NonPublic)!;

	public void Patch(HastyControlsMod mod)
	{
		On.Grapple.Activate += (orig, self) =>
		{
			Player player = (Player)playerField.GetValue(self);
			mod.Events.PlayerGrappleAbilityUsed?.Invoke(player);

			orig(self);
		};

		On.Grapple.EndGrapple += (orig, self) =>
		{
			Player player = (Player)playerField.GetValue(self);
			mod.Events.PlayerGrappleAbilityFinished?.Invoke(player);

			orig(self);
		};
	}
}