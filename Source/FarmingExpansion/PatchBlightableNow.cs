using HarmonyLib;
using RimWorld;

namespace FarmingExpansion;

[HarmonyPatch(typeof(Plant))]
[HarmonyPatch("BlightableNow", MethodType.Getter)]
internal static class PatchBlightableNow
{
    [HarmonyPostfix]
    public static void ChemicalsPreventBlight(Plant __instance, ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(__instance))
        {
            __result = false;
        }

        if (ChemicalAddedCheck.DDTAppliedOnto.Contains(__instance))
        {
            __result = false;
        }

        if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(__instance))
        {
            __result = false;
        }
    }
}