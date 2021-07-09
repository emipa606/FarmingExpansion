using HarmonyLib;
using RimWorld;
using Verse;

namespace FarmingExpansion
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch("TryFindBestFoodSourceFor")]
    internal static class PatchFoodUtility
    {
        [HarmonyPostfix]
        public static void ForceBugsToEatCrops(ref Pawn eater, ref Thing foodSource, ref bool __result)
        {
            if (eater.def.defName != "FE_CropEaterInsect" || foodSource == null)
            {
                return;
            }

            Log.Message($"{foodSource} - {eater}");
            if (!ChemicalAddedCheck.PesticideAppliedOnto.Contains((Plant) foodSource) &&
                !ChemicalAddedCheck.DDTAppliedOnto.Contains((Plant) foodSource) &&
                !ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains((Plant) foodSource))
            {
                return;
            }

            foodSource = null;
            __result = false;
        }
    }
}