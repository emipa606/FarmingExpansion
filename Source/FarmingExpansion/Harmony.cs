using HarmonyLib;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using System.Text;
using System;

namespace FarmingExpansion
{

    // This changes the growth rate of the plant
    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("GrowthRate", MethodType.Getter)]
    static class PatchChemicalGrowth
    {
        [HarmonyPostfix]
        public static void ImprovePlantSpeed(Plant __instance, ref float __result)
        {
            float speedChange = 0;
            if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(__instance))
            {
                speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Fungicide")).PercentagePointSpeedChange;
            }
            if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(__instance))
            {
                speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Herbicide")).PercentagePointSpeedChange;
            }
            if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(__instance))
            {
                speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Pesticide")).PercentagePointSpeedChange;
            }
            if (ChemicalAddedCheck.DDTAppliedOnto.Contains(__instance))
            {
                speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_DDT")).PercentagePointSpeedChange;
            }
            if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(__instance))
            {
                speedChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_ChemicalSpray")).PercentagePointSpeedChange;
            }
            __result *= 1 + speedChange;
        }
    }

    // This changes the yeld rate of the plant
    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("YieldNow")]
    static class PatchChemicalYield
    {
        [HarmonyPostfix]
        public static void ImprovePlantYield(Plant __instance, ref int __result)
        {
            if (__result == 0)
            {
                return;
            }
            float yieldChange = 0;
            if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(__instance))
            {
                yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Fungicide")).PercentagePointYeildChange;
            }
            if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(__instance))
            {
                yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Herbicide")).PercentagePointYeildChange;
            }
            if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(__instance))
            {
                yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_Pesticide")).PercentagePointYeildChange;
            }
            if (ChemicalAddedCheck.DDTAppliedOnto.Contains(__instance))
            {
                yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_DDT")).PercentagePointYeildChange;
            }
            if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(__instance))
            {
                yieldChange = ((FarmingExpansion_Chemical)DefDatabase<ThingDef>.GetNamed("FE_ChemicalSpray")).PercentagePointYeildChange;
            }
            __result = GenMath.RoundRandom(__result * (1 + yieldChange));
        }
    }

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("BlightableNow", MethodType.Getter)]
    static class PatchBlightableNow
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

    [HarmonyPatch(typeof(Zone_Growing))]
    [HarmonyPatch("GetGizmos")]
    static class PatchChemicalsAppliedButton
    {

        [HarmonyPostfix]
        public static void AddChemicalAllowedButton(Zone_Growing __instance, ref IEnumerable<Gizmo> __result)
        {

            List<Gizmo> newList = new List<Gizmo>();

            foreach (Gizmo gizmo in __result)
            {

                newList.Add(gizmo);

            }
            newList.Add(new Command_Toggle
            {
                defaultLabel = "Apply Chemicals",
                defaultDesc = "The pawns will, if possible, apply chemicals onto this zone's plants",
                hotKey = KeyBindingDefOf.Misc1,
                isActive = (() =>
                {
                    if (UseChemicalWorkGiver.IsGrowingZonesWithChemicalList.Contains(__instance))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/ApplyChemicals", true),
                toggleAction = delegate
                {
                    if (UseChemicalWorkGiver.IsGrowingZonesWithChemicalList.Contains(__instance))
                    {
                        UseChemicalWorkGiver.IsGrowingZonesWithChemicalList.Remove(__instance);
                    }
                    else
                    {
                        UseChemicalWorkGiver.IsGrowingZonesWithChemicalList.Add(__instance);
                    }
                }
            });

            __result = newList;
        }
    }

    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("GetInspectString")]
    static class PatchPlantInspectString
    {
        [HarmonyPostfix]
        public static void AddChemicalInfoToInspectString(Plant __instance, ref string __result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(__instance))
            {
                stringBuilder.AppendLine("FE_FungicideInfo".Translate());
            }
            if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(__instance))
            {
                stringBuilder.AppendLine("FE_HerbicideInfo".Translate());
            }
            if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(__instance))
            {
                stringBuilder.AppendLine("FE_PesticideInfo".Translate());
            }
            if (ChemicalAddedCheck.DDTAppliedOnto.Contains(__instance))
            {
                stringBuilder.AppendLine("FE_DDTInfo".Translate());
            }
            if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(__instance))
            {
                stringBuilder.AppendLine("FE_ChemicalSprayInfo".Translate());
            }
            if (stringBuilder.Length > 0)
            {
                __result = __result + Environment.NewLine + stringBuilder.ToString().TrimEndNewlines();
            }
        }
    }


    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch("TryFindBestFoodSourceFor")]
    static class PatchFoodUtility
    {

        [HarmonyPostfix]
        public static void ForceBugsToEatCrops(ref Pawn eater, ref Thing foodSource, ref bool __result)
        {
            if (eater.def.defName != "FE_CropEaterInsect" || foodSource == null)
            {
                return;
            }
            Log.Message($"{foodSource} - {eater}");
            if (ChemicalAddedCheck.PesticideAppliedOnto.Contains((Plant)foodSource) ||
                ChemicalAddedCheck.DDTAppliedOnto.Contains((Plant)foodSource) ||
                ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains((Plant)foodSource))
            {
                foodSource = null;
                __result = false;
            }
        }
    }
}
