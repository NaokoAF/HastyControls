using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(Player))]
internal static class PlayerEventsPatch
{
	[HarmonyPatch("Awake")]
	[HarmonyPostfix]
	static void AwakePostfix(Player __instance)
	{
		__instance.takeDamageAction += (damage, source, effect) => Mod.Events.PlayerDamaged?.Invoke(__instance, damage, source, effect);
		__instance.pickUpCoinAction += () => Mod.Events.PlayerSparkPickedUp?.Invoke(__instance);
		__instance.HealthChangedAction += (health) => Mod.Events.PlayerHealthChanged?.Invoke(__instance, health);
	}

	[HarmonyPatch("AddResource")]
	[HarmonyPostfix]
	static void AddResourcePostfix(Player __instance, int amount, EffectSource source)
	{
		Mod.Events.PlayerResourceReceived?.Invoke(__instance, source, amount);
	}

	[HarmonyPatch("SetEnergy")]
	[HarmonyPostfix]
	static void SetEnergyPostfix(Player __instance)
	{
		Mod.Events.PlayerEnergyChanged?.Invoke(__instance, __instance.data.energy);
	}
}
