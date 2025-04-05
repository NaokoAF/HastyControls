using HarmonyLib;
using Zorro.Localization;

namespace HastyControls.Core.Patches;

[HarmonyPatch(typeof(LocalizeUIText))]
internal static class LocalizeUITextPatch
{
	[HarmonyPatch("OnStringChanged")]
	[HarmonyPostfix]
	static void OnStringChangedPostfix(LocalizeUIText __instance)
	{
		if (__instance.String == null)
			return;

		string collection = __instance.String.TableReference.TableCollectionName;
		if (collection != ModInfo.Guid)
			return;

		string entry = __instance.String.TableEntryReference.Key;
		__instance.Text.text = entry;
	}
}
