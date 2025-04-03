using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(Ability_Fly))]
internal static class AbilityFlyPatch
{
	[HarmonyPatch("Update")]
	[HarmonyPrefix]
	static void UpdatePrefix(Ability_Fly __instance)
	{
		var player = PlayerCharacter.localPlayer;
		bool ground = player.data.mostlyGrounded;
		float cost = ground ? __instance.groundCost : __instance.airCost;
		if (player.input.abilityWasPressed && player.player.data.energy >= cost)
		{
			Mod.Events.PlayerFlyAbilityUsed?.Invoke(ground);
		}
	}
}
