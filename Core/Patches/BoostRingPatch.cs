using HarmonyLib;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(TriggerEffect))]
internal static class BoostRingPatch
{
	[HarmonyPatch("OnTriggerStay")]
	[HarmonyPrefix]
	static void OnTriggerStayPrefix(TriggerEffect __instance, ref bool __state, ref bool ___used)
	{
		__state = ___used;
	}

	[HarmonyPatch("OnTriggerStay")]
	[HarmonyPostfix]
	static void OnTriggerStayPostfix(TriggerEffect __instance, ref bool __state, ref bool ___used)
	{
		bool used = ___used;
		if (used && !__state)
		{
			AddVariable_Boost? boostEffect = FindOfType<AddVariable_Boost>(__instance.effects);
			if (boostEffect != null)
			{
				Mod.Events.PlayerBoostRingPassed?.Invoke(boostEffect.addSpeed);
			}
		}
	}

	static T? FindOfType<T>(List<ItemEffect> effects)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			if (effects[i] is T type)
				return type;
		}
		return default;
	}
}
