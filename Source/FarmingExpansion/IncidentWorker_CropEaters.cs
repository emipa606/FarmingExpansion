using System.Collections.Generic;
using RimWorld;
using Verse;

namespace FarmingExpansion;

public class IncidentWorker_CropEaters : IncidentWorker
{
    private const float MaxDaysToGrown = 16f;

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var list = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
        var b = false;
        TargetInfo bugs = null;

        var plants = new List<Plant>();

        for (var i = list.Count - 1; i >= 0; i--)
        {
            var plant = (Plant)list[i];
            if (map.Biome.CommonalityOfPlant(plant.def) != 0f)
            {
                continue;
            }

            if (plant.def.plant.growDays > MaxDaysToGrown)
            {
                continue;
            }

            if (plant.LifeStage != PlantLifeStage.Growing && plant.LifeStage != PlantLifeStage.Mature)
            {
            }

            if (ChemicalAddedCheck.PesticideAppliedOnto.Contains(plant))
            {
                continue;
            }

            if (ChemicalAddedCheck.DDTAppliedOnto.Contains(plant))
            {
                continue;
            }

            if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(plant))
            {
                continue;
            }

            plants.Add(plant);
        }

        if (plants.Count == 0)
        {
            return false;
        }

        var spawnLimit = plants.Count / 4;
        if (spawnLimit < 3)
        {
            spawnLimit = 2;
        }

        if (spawnLimit > 12)
        {
            spawnLimit = 12;
        }

        for (var i = 0; i < spawnLimit; i++)
        {
            var plant = plants.RandomElement();
            plants.Remove(plant);
            //bool flag2 = false;
            //if (spawnedBugs >= spawnLimit) break;
            //if (spawnedBugs == 0) { flag2 = true; }

            var pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("FE_CropEaterInsect"));
            bugs = new TargetInfo(plant.Position, plant.Map);
            GenSpawn.Spawn(pawn, plant.Position, plant.Map, Rot4.Random);
            b = true;
        }

        if (!b)
        {
            return false;
        }

        Find.LetterStack.ReceiveLetter("Pests!",
            "Some crop-eating bugs have found their way into your growing zones. They will eat all the crops they can find if they aren't dealt with.",
            LetterDefOf.NegativeEvent, bugs);
        return true;
    }
}