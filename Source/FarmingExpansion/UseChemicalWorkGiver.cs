using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FarmingExpansion
{
    public class UseChemicalWorkGiver : WorkGiver_Grower
    {
        public static List<Zone_Growing> IsGrowingZonesWithChemicalList = new List<Zone_Growing>();
        private Thing closestChemical;

        private int jobCount;

        // Decides how close colonist must be to the plant to do action
        public override PathEndMode PathEndMode => PathEndMode.Touch;

        /// <summary>
        ///     Extra parameters on whether the plant on the cell is suitable for using chemicals on
        /// </summary>
        public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            var plant = c.GetPlant(pawn.Map);
            if (plant == null)
            {
                return false;
            }

            if (!pawn.CanReserve(plant))
            {
                return false;
            }

            if (!(c.GetZone(pawn.Map) is Zone_Growing))
            {
                return false;
            }

            if (!IsGrowingZonesWithChemicalList.Contains(c.GetZone(pawn.Map) as Zone_Growing))
            {
                return false;
            }

            if (plant.def != (plant.Position.GetZone(pawn.Map) as Zone_Growing)?.GetPlantDefToGrow())
            {
                return false;
            }

            foreach (var chemicalDef in new List<string>
                {"FE_ChemicalSpray", "FE_DDT", "FE_Pesticide", "FE_Herbicide", "FE_Fungicide"})
            {
                closestChemical = FindChemical(pawn, DefDatabase<ThingDef>.GetNamed(chemicalDef));
                if (closestChemical == null)
                {
                    continue;
                }

                if (Utility.ChemicalOkayToUse(chemicalDef, plant))
                {
                    break;
                }
            }

            if (closestChemical == null)
            {
                return false;
            }

            if (!pawn.CanReserve(closestChemical))
            {
                return false;
            }

            if (c.GetZone(pawn.Map) is Zone_Growing growingZone)
            {
                var possibleWorkCellsInZone = growingZone.Cells.Count;

                // Checks how many plants other colonists are in the process of applying chemicals on within the same zone. This prevents pawns from trying to apply pesticide
                // when other pawns already have enough pesticide for the whole zone.
                bool validator(Job job)
                {
                    if (job.targetA.Cell.GetZone(pawn.Map) == growingZone)
                    {
                        return true;
                    }

                    return false;
                }

                void action(Job job, Pawn otherColonist)
                {
                    possibleWorkCellsInZone -= PlantsOtherColonistsApplyPesticideOnto(otherColonist, job.count);
                }

                Utility.DoOnOtherPawnsWithSameJobQueuedOrActive(pawn, typeof(UseChemicalJobDriver), validator, action);

                foreach (var cellInZone in growingZone.Cells)
                {
                    var plantOnCellInZone = cellInZone.GetPlant(pawn.Map);

                    // Removes unusable cells from the count of possible work cells
                    if (plantOnCellInZone != null && plantOnCellInZone.def == growingZone.GetPlantDefToGrow() &&
                        Utility.ChemicalOkayToUse(closestChemical.def.defName, plantOnCellInZone))
                    {
                        continue;
                    }

                    possibleWorkCellsInZone--;
                }

                if (possibleWorkCellsInZone < 0)
                {
                    possibleWorkCellsInZone = 0;
                }

                // Apply chemicals on as many tiles as possible to the best of the pawn's ability
                jobCount = possibleWorkCellsInZone >= pawn.carryTracker.AvailableStackSpace(closestChemical.def)
                    ? pawn.carryTracker.AvailableStackSpace(closestChemical.def)
                    : possibleWorkCellsInZone;
            }

            if (jobCount > 0)
            {
                return true;
            }

            return false;
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 cell, bool forced = false)
        {
            if (wantedPlantDef == null)
            {
                wantedPlantDef = CalculateWantedPlantDef(cell, pawn.Map);
            }

            return new Job(DefDatabase<JobDef>.GetNamed("FE_UseChemicalJobDef"), cell.GetPlant(pawn.Map),
                closestChemical)
            {
                count = jobCount
            };
        }

        /// <summary>
        ///     Returns the amount of plants in one growing zone that other pawns will apply chemicals onto
        /// </summary>
        /// <param name="otherColonist"></param>
        /// <param name="jobCountOfOtherColonist">
        ///     This is the count of how many pesticide items the pawn will collect before doing
        ///     the field work
        /// </param>
        /// <returns>How many plants the pawn will apply chemicals onto within this job</returns>
        private int PlantsOtherColonistsApplyPesticideOnto(Pawn otherColonist, int jobCountOfOtherColonist)
        {
            // If there are yet pesticide items to carry...
            if (jobCountOfOtherColonist == 0)
            {
                return otherColonist.carryTracker.CarriedThing.stackCount;
            }

            if (otherColonist.carryTracker.CarriedThing == null ||
                jobCountOfOtherColonist == otherColonist.carryTracker.CarriedThing.stackCount)
            {
                return jobCountOfOtherColonist;
            }

            return jobCountOfOtherColonist + otherColonist.carryTracker.CarriedThing.stackCount;

            // If the pawn has aldready collected all chemicals it will need, return the amount collected (one item collected equals one plant with chemicals applied onto)
        }

        protected override bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
        {
            if (!(settable is Zone_Growing zone_Growing))
            {
                return false;
            }

            var c = zone_Growing.Cells[0];
            wantedPlantDef = CalculateWantedPlantDef(c, pawn.Map);
            return wantedPlantDef != null;
        }

        /// <summary>
        ///     Finds and returns the closest, reachable, unforbidden, pesticide
        /// </summary>
        private Thing FindChemical(Pawn pawn, ThingDef chemicalDef)
        {
            bool predicate(Thing x)
            {
                return !x.IsForbidden(pawn) && pawn.CanReserve(x);
            }

            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(chemicalDef),
                PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, predicate);
        }
    }
}