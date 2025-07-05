using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace FarmingExpansion;

[HarmonyPatch(typeof(Zone_Growing), nameof(Zone_Growing.GetGizmos))]
internal static class Zone_Growing_GetGizmos
{
    public static void Postfix(Zone_Growing __instance, ref IEnumerable<Gizmo> __result)
    {
        var newList = new List<Gizmo>();

        foreach (var gizmo in __result)
        {
            newList.Add(gizmo);
        }

        newList.Add(new Command_Toggle
        {
            defaultLabel = "Apply Chemicals",
            defaultDesc = "The pawns will, if possible, apply chemicals onto this zone's plants",
            hotKey = KeyBindingDefOf.Misc1,
            isActive = () => UseChemicalWorkGiver.IsGrowingZonesWithChemicalList.Contains(__instance),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/ApplyChemicals"),
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