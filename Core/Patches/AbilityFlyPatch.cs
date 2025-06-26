using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(Ability_Fly))]
internal static class AbilityFlyPatch
{
	[HarmonyPatch("Update")]
	[HarmonyPrefix]
	static void UpdatePrefix(Ability_Fly __instance, PlayerCharacter ___player)
	{
		bool ground = ___player.data.mostlyGrounded;
		float cost = ground ? __instance.groundCost : __instance.airCost;
		if (___player.input.abilityWasPressed && ___player.player.data.energy >= cost)
		{
			Mod.Events.PlayerFlyAbilityUsed?.Invoke(___player.player, ground);
		}
	}
}
