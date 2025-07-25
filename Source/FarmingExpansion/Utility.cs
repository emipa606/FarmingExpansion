﻿using System;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace FarmingExpansion;

//While this Utility class is not strictly needed at the moment, it will become more useful in the future if more content is added
internal static class Utility
{
    public static void DoOnOtherPawnsWithSameJobQueuedOrActive(Pawn pawn, Type jobType,
        Func<Job, bool> extraValidator, Action<Job, Pawn> action)
    {
        foreach (var otherColonist in pawn.Map.mapPawns.FreeColonists)
        {
            if (otherColonist == pawn)
            {
                continue;
            }

            if (otherColonist.jobs.curDriver.GetType() == jobType &&
                (extraValidator == null || extraValidator(otherColonist.jobs.curDriver.job)))
            {
                action(otherColonist.jobs.curDriver.job, otherColonist);
            }

            // This Linq statement returns a list containing all the apply pesticide jobs. It is a list because pawns player-driven priorisation commands can queue up
            // jobs meaning that there can be several apply pesticide jobs
            var queuedJobsOfSpecifiedType =
                otherColonist.jobs.jobQueue.Where(j => j.job.def.driverClass == jobType);
            if (!queuedJobsOfSpecifiedType.Any())
            {
                continue;
            }

            foreach (var queuedJob in queuedJobsOfSpecifiedType)
            {
                if (extraValidator != null && !extraValidator(queuedJob.job))
                {
                    continue;
                }

                action(queuedJob.job, otherColonist);
            }
        }
    }

    public static bool ChemicalOkayToUse(string chemicalDefName, Plant plant)
    {
        switch (chemicalDefName)
        {
            case "FE_Fungicide":
                if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.DDTAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(plant))
                {
                    return false;
                }

                break;
            case "FE_Herbicide":
                if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.DDTAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(plant))
                {
                    return false;
                }

                break;
            case "FE_Pesticide":
                if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.DDTAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(plant))
                {
                    return false;
                }

                break;
            case "FE_DDT":
            case "FE_ChemicalSpray":
                if (ChemicalAddedCheck.FungicideAppliedOnto.Contains(plant) &&
                    ChemicalAddedCheck.HerbicideAppliedOnto.Contains(plant) &&
                    ChemicalAddedCheck.PesticideAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.DDTAppliedOnto.Contains(plant))
                {
                    return false;
                }

                if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(plant))
                {
                    return false;
                }

                break;
        }

        return true;
    }
}