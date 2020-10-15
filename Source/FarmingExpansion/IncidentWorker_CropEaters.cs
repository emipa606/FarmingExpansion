using System.Collections.Generic;
using Verse;
using RimWorld;

namespace FarmingExpansion
{
    public class IncidentWorker_CropEaters : IncidentWorker
    {
        private const float MaxDaysToGrown = 16f;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
            bool flag = false;
            TargetInfo bugs = null;

            int spawnedBugs = 0;

            List<Plant> plants = new List<Plant>();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                Plant plant = (Plant)list[i];
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
            if(plants.Count == 0)
                return false;

            int spawnLimit = plants.Count / 4;
            if (spawnLimit < 3) spawnLimit = 2;
            if (spawnLimit > 12) spawnLimit = 12;

            for (int i = 0; i < spawnLimit; i++)
            {
                Plant plant = plants.RandomElement();
                plants.Remove(plant);
                //bool flag2 = false;
                //if (spawnedBugs >= spawnLimit) break;
                //if (spawnedBugs == 0) { flag2 = true; }

                Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("FE_CropEaterInsect"), null);
                bugs = new TargetInfo(plant.Position, plant.Map);
                GenSpawn.Spawn(pawn, plant.Position, plant.Map, Rot4.Random);
                flag = true;
                spawnedBugs += 1;
            }

            if (!flag)
            {
                return false;
            }
            Find.LetterStack.ReceiveLetter("Pests!", "Some crop-eating bugs have found their way into your growing zones. They will eat all the crops they can find if they aren't dealt with.", LetterDefOf.NegativeEvent, bugs);
            return true;
        }

    }
}
