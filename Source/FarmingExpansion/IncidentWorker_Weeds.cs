using System.Collections.Generic;
using Verse;
using RimWorld;
using System.Linq;
using System;

namespace FarmingExpansion
{
    public class IncidentWorker_Weeds : IncidentWorker_CropBlight
    {
        private static readonly string WeedDefName = "FE_Weed";

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            float num = RadiusFactorPerPointsCurve.Evaluate(parms.points);
            if (!TryFindRandomWeedablePlant(map, out Plant plant))
            {
                return false;
            }
            Room room = plant.GetRoom(RegionType.Set_Passable);
            int i = 0;
            int num2 = GenRadial.NumCellsInRadius(Radius * num);
            while (i < num2)
            {
                IntVec3 intVec = plant.Position + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && intVec.GetRoom(map, RegionType.Set_Passable) == room)
                {
                    Plant firstWeedableNowPlant = GetFirstWeedableNowPlant(intVec, map);
                    if (firstWeedableNowPlant != null && firstWeedableNowPlant.def == plant.def && Rand.Chance(this.WeedChance(firstWeedableNowPlant.Position, plant.Position, num)))
                    {
                        MakePlantWeed(firstWeedableNowPlant.Position, firstWeedableNowPlant.Map);
                    }
                }
                i++;
            }
            SendStandardLetter("FE_LetterLabelWeed".Translate(new NamedArgument(plant.def, "PLANTDEF")), "FE_LetterWeed".Translate(new NamedArgument(plant.def, "PLANTDEF")), LetterDefOf.NegativeEvent, parms, new TargetInfo(plant.Position, map, false), Array.Empty<NamedArgument>());
            return true;
        }

        public static void MakePlantWeed(IntVec3 cell, Map map)
        {
            var plant = cell.GetPlant(map);
            plant.Destroy();
            GenSpawn.Spawn(weedDef, cell, map, WipeMode.Vanish);
        }

        private float WeedChance(IntVec3 c, IntVec3 root, float radiusFactor)
        {
            float x = c.DistanceTo(root) / radiusFactor;
            return WeedChancePerRadius.Evaluate(x);
        }

        public static Plant GetFirstWeedableNowPlant(IntVec3 c, Map map)
        {
            Plant plant = c.GetPlant(map);
            if (plant != null && IsWeedabe(plant))
            {
                return plant;
            }
            return null;
        }
        public static bool IsWeedabe(Plant plant)
        {
            if (plant.def.defName == WeedDefName) 
                return false;
            if (!plant.def.plant.Blightable) 
                return false;
            if (!plant.sown) 
                return false;
            if (plant.LifeStage == PlantLifeStage.Sowing) 
                return false;
            if (plant.Map.Biome.AllWildPlants.Contains(plant.def)) 
                return false;
            if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(plant))
                return false;
            if (ChemicalAddedCheck.DDTAppliedOnto.Contains(plant))
                return false;
            if (ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(plant))
                return false;
            return true;
        }

        private bool TryFindRandomWeedablePlant(Map map, out Plant plant)
        {
            bool result = (from x in map.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
                           where IsWeedabe((Plant)x)
                           select x).TryRandomElement(out Thing thing);
            plant = (Plant)thing;
            return result;
        }

        private static readonly ThingDef weedDef = DefDatabase<ThingDef>.GetNamedSilentFail(WeedDefName);

        private const float Radius = 3f;

        // Token: 0x0400254D RID: 9549
        private static readonly SimpleCurve WeedChancePerRadius = new SimpleCurve
        {
            {
                new CurvePoint(0f, 1f),
                true
            },
            {
                new CurvePoint(8f, 1f),
                true
            },
            {
                new CurvePoint(11f, 0.3f),
                true
            }
        };

        // Token: 0x0400254E RID: 9550
        private static readonly SimpleCurve RadiusFactorPerPointsCurve = new SimpleCurve
        {
            {
                new CurvePoint(25f, 0.6f),
                true
            },
            {
                new CurvePoint(100f, 1f),
                true
            },
            {
                new CurvePoint(500f, 2f),
                true
            }
        };

    }
}
