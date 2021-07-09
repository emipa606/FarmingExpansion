using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FarmingExpansion
{
    public class UseChemicalJobDriver : JobDriver
    {
        private float applyChemicalWorkDone;

        private FarmingExpansion_Chemical ChemicalDef;

        // The targeted plant
        private Plant Plant => (Plant) job.GetTarget(TargetPlant).Thing;

        //A generic representation of the objects used - only used as a parameter
        // TargetIndex.A & B can be used directly but it can be a bit ambiguous
        private TargetIndex TargetPlant => TargetIndex.A;
        private TargetIndex TargetChemical => TargetIndex.B;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // Reserve the plant and the pesticide
            if (pawn.Reserve(job.targetA, job) && pawn.Reserve(job.targetB, job))
            {
                return true;
            }

            return false;
        }

        // Explains how the job will be done - step by step (every yield keyword equals one "step")
        protected override IEnumerable<Toil> MakeNewToils()
        {
            var pesticideReserved = Toils_Reserve.Reserve(TargetChemical);
            yield return pesticideReserved;

            // Fail conditions - prevents having to write long Lambdas
            yield return Toils_Goto.GotoThing(TargetChemical, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden(TargetChemical).FailOnSomeonePhysicallyInteracting(TargetChemical);

            // The jobCount here has to decrement or else there is no way for the next toil to figure out how many more pesticide items should be gathered
            yield return Toils_Haul.StartCarryThing(TargetChemical, false, true)
                .FailOnDestroyedNullOrForbidden(TargetChemical);
            yield return Toils_Reserve.Release(TargetChemical);

            // Gather other pesticide items if neccessary
            yield return Toils_Haul.CheckForGetOpportunityDuplicate(pesticideReserved, TargetChemical, TargetIndex.None,
                true);

            // Go to the targeted plant
            var goToPlantCell = Toils_Goto.GotoCell(TargetPlant, PathEndMode.Touch);
            goToPlantCell.AddFinishAction(delegate { job.count = job.GetTarget(TargetChemical).Thing.stackCount; });
            yield return goToPlantCell;

            // The actual 'apply pesticide' action comes below
            var toil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Never,

                initAction = delegate
                {
                    TargetThingA = Plant;
                    ChemicalDef = (FarmingExpansion_Chemical) job.GetTarget(TargetChemical).Thing.def;
                },

                tickAction = delegate
                {
                    // Increases Growing skill a little (sowing is 0.11f)
                    pawn.skills.Learn(SkillDefOf.Plants, 0.02f);

                    applyChemicalWorkDone += 6 * pawn.GetStatValue(StatDefOf.PlantWorkSpeed);
                    if (applyChemicalWorkDone >= Plant.def.plant.sowWork)
                    {
                        ReadyForNextToil();
                    }
                }
            };

            toil.AddFinishAction(delegate
            {
                // This checks to see if the toil is finished because the work is done and not due to outside interferance
                if (!(applyChemicalWorkDone >= Plant.def.plant.sowWork))
                {
                    return;
                }

                var chemical = pawn.carryTracker.CarriedThing.def.defName;
                // Decreasing the stackcount of an object with 1 in stackcount leads to an error
                if (pawn.carryTracker.CarriedThing.stackCount == 1)
                {
                    pawn.carryTracker.CarriedThing.Destroy(DestroyMode.Cancel);
                }
                else
                {
                    pawn.carryTracker.CarriedThing.stackCount--;
                }

                if (job.count > 0)
                {
                    job.count--;
                }

                switch (chemical)
                {
                    case "FE_Fungicide":
                        ChemicalAddedCheck.FungicideAppliedOnto.Add(Plant);
                        break;
                    case "FE_Herbicide":
                        ChemicalAddedCheck.HerbicideAppliedOnto.Add(Plant);
                        break;
                    case "FE_Pesticide":
                        ChemicalAddedCheck.PesticideAppliedOnto.Add(Plant);
                        break;
                    case "FE_DDT":
                        ChemicalAddedCheck.DDTAppliedOnto.Add(Plant);
                        if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(Plant))
                        {
                            ChemicalAddedCheck.FungicideAppliedOnto.Remove(Plant);
                        }

                        if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(Plant))
                        {
                            ChemicalAddedCheck.HerbicideAppliedOnto.Remove(Plant);
                        }

                        if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(Plant))
                        {
                            ChemicalAddedCheck.PesticideAppliedOnto.Remove(Plant);
                        }

                        break;
                    case "FE_ChemicalSpray":
                        ChemicalAddedCheck.ChemicalSprayAppliedOnto.Add(Plant);
                        if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(Plant))
                        {
                            ChemicalAddedCheck.FungicideAppliedOnto.Remove(Plant);
                        }

                        if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(Plant))
                        {
                            ChemicalAddedCheck.HerbicideAppliedOnto.Remove(Plant);
                        }

                        if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(Plant))
                        {
                            ChemicalAddedCheck.PesticideAppliedOnto.Remove(Plant);
                        }

                        break;
                }

                applyChemicalWorkDone = 0f;
            });

            toil.FailOnDespawnedNullOrForbidden(TargetPlant);
            toil.FailOnCannotTouch(TargetPlant, PathEndMode.Touch);
            toil.WithEffect(EffecterDefOf.Sow, TargetPlant);
            toil.WithProgressBar(TargetIndex.A, () => applyChemicalWorkDone / Plant.def.plant.sowWork, true);
            toil.PlaySustainerOrSound(() => SoundDefOf.Interact_Sow);

            yield return toil;
            yield return Toils_Reserve.Release(TargetPlant);
            yield return ApplyOntoRandomNearbyPlantNext();
            yield return Toils_Reserve.Reserve(TargetPlant);

            //Go directly to another plant and apply chemical on it
            yield return Toils_Jump.Jump(goToPlantCell);
        }

        /// <summary>
        ///     Finds a plant, which is preferably as close to the pawn as possible but at least within the same zone, and sets it
        ///     as the new job target. This results in the pawn going plant to plant
        ///     as it applies chemicals without having to start the job over and collect a new chemical item to replace the used
        ///     one.
        /// </summary>
        private Toil ApplyOntoRandomNearbyPlantNext()
        {
            var toil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Instant,

                initAction = delegate
                {
                    if (pawn.carryTracker.CarriedThing == null ||
                        pawn.carryTracker.CarriedThing.def != job.targetB.Thing.def)
                    {
                        EndJobWith(JobCondition.Incompletable);
                    }
                    else
                    {
                        // Different checks on the cell
                        bool predicate(IntVec3 cellBeingChecked)
                        {
                            if (cellBeingChecked.GetPlant(pawn.Map) == null)
                            {
                                return false;
                            }

                            if (!(cellBeingChecked.GetZone(pawn.Map) is Zone_Growing))
                            {
                                return false;
                            }

                            if (job.GetTarget(TargetPlant).Cell.GetZone(pawn.Map) !=
                                cellBeingChecked.GetZone(pawn.Map) as Zone_Growing)
                            {
                                return false;
                            }

                            if (!UseChemicalWorkGiver.IsGrowingZonesWithChemicalList.Contains(
                                cellBeingChecked.GetZone(pawn.Map) as Zone_Growing))
                            {
                                return false;
                            }

                            if (cellBeingChecked.GetPlant(pawn.Map).def !=
                                (job.GetTarget(TargetPlant).Cell.GetZone(pawn.Map) as Zone_Growing)
                                ?.GetPlantDefToGrow())
                            {
                                return false;
                            }

                            if (!pawn.CanReserveAndReach(cellBeingChecked.GetPlant(pawn.Map), PathEndMode.ClosestTouch,
                                pawn.NormalMaxDanger()))
                            {
                                return false;
                            }

                            var chemicalDefName = pawn.carryTracker.CarriedThing.def.defName;
                            var plant = cellBeingChecked.GetPlant(pawn.Map);
                            return Utility.ChemicalOkayToUse(chemicalDefName, plant);
                        }

                        // Preferably have the next plant be in touch range, or one tile away, of the pawn
                        if (CellFinder.TryFindRandomCellNear(pawn.Position, pawn.Map, 1, predicate,
                            out var newPlantCellLocation) || CellFinder.TryFindRandomCellNear(pawn.Position, pawn.Map,
                            2, predicate, out newPlantCellLocation))
                        {
                            job.SetTarget(TargetIndex.A, newPlantCellLocation.GetPlant(pawn.Map));
                            job.SetTarget(TargetIndex.B, pawn.carryTracker.CarriedThing);

                            // If the above was not possible, find another plant in the zone which chemical have not been applied to
                        }
                        else
                        {
                            foreach (var cell in job.GetTarget(TargetPlant).Cell.GetZone(pawn.Map).Cells)
                            {
                                if (!predicate(cell))
                                {
                                    continue;
                                }

                                job.SetTarget(TargetIndex.A, cell.GetPlant(pawn.Map));
                                job.SetTarget(TargetIndex.B, pawn.carryTracker.CarriedThing);
                                return;
                            }

                            EndJobWith(JobCondition.Incompletable);
                        }
                    }
                }
            };
            return toil;
        }
    }
}