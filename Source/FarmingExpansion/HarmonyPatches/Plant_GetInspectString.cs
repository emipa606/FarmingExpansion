using System;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FarmingExpansion;

[HarmonyPatch(typeof(Plant), nameof(Plant.GetInspectString))]
internal static class Plant_GetInspectString
{
    public static void Postfix(Plant __instance, ref string __result)
    {
        var stringBuilder = new StringBuilder();
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