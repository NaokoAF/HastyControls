using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(Player))]
internal static class PlayerEventsPatch
{
	[HarmonyPatch("Awake")]
	[HarmonyPostfix]
	static void AwakePostfix(Player __instance)
	{
		__instance.takeDamageAction += (damage, source, effect) => Mod.Events.PlayerDamaged?.Invoke(damage, source, effect);
		__instance.pickUpCoinAction += () => Mod.Events.PlayerSparkPickedUp?.Invoke();
		__instance.HealthChangedAction += (health) => Mod.Events.PlayerHealthChanged?.Invoke(health);
	}

	[HarmonyPatch("AddResource")]
	[HarmonyPostfix]
	static void AddResourcePostfix(Player __instance, int amount, EffectSource source)
	{
		Mod.Events.PlayerResourceReceived?.Invoke(source, amount);
	}

	[HarmonyPatch("SetEnergy")]
	[HarmonyPostfix]
	static void SetEnergyPostfix(Player __instance)
	{
		Mod.Events.PlayerEnergyChanged?.Invoke(__instance.data.energy);
	}
}
