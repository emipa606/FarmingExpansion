using HarmonyLib;
using RimWorld;
using Verse;

namespace FarmingExpansion;

// This changes the growth rate of the plant
[HarmonyPatch(typeof(Plant))]
[HarmonyPatch("GrowthRate", MethodType.Getter)]
internal static class PatchChemicalGrowth
{
    [HarmonyPostfix]
    public static void ImprovePlantSpeed(Plant __instance, ref float __result)
    {
        float speedChange = 0;
        if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(__instance))
        {
            speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Fungicide"))
                .PercentagePointSpeedChange;
        }

        if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(__instance))
        {
            speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Herbicide"))
                .PercentagePointSpeedChange;
        }

        if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(__instance))
        {
            speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Pesticide"))
                .PercentagePointSpeedChange;
        }

        if (ChemicalAddedCheck.DDTAppliedOnto.Contains(__instance))
        {
            speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_DDT"))
                .PercentagePointSpeedChange;
        }

        if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(__instance))
        {
            speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_ChemicalSpray"))
                .PercentagePointSpeedChange;
        }

        __result *= 1 + speedChange;
    }
}

// This changes the yeld rate of the plant