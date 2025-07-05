using HarmonyLib;
using RimWorld;
using Verse;

namespace FarmingExpansion;

[HarmonyPatch(typeof(Plant), nameof(Plant.YieldNow))]
internal static class Plant_YieldNow
{
    public static void Postfix(Plant __instance, ref int __result)
    {
        if (__result == 0)
        {
            return;
        }

        float yieldChange = 0;
        if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(__instance))
        {
            yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Fungicide"))
                .PercentagePointYeildChange;
        }

        if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(__instance))
        {
            yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Herbicide"))
                .PercentagePointYeildChange;
        }

        if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(__instance))
        {
            yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Pesticide"))
                .PercentagePointYeildChange;
        }

        if (ChemicalAddedCheck.DDTAppliedOnto.Contains(__instance))
        {
            yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_DDT"))
                .PercentagePointYeildChange;
        }

        if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(__instance))
        {
            yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_ChemicalSpray"))
                .PercentagePointYeildChange;
        }

        __result = GenMath.RoundRandom(__result * (1 + yieldChange));
    }
}