using System.Linq;
using RimWorld;
using Verse;

namespace FarmingExpansion;

public class IncidentWorker_Weeds : IncidentWorker_CropBlight
{
    private const float Radius = 3f;
    private static readonly string WeedDefName = "FE_Weed";

    private static readonly ThingDef weedDef = DefDatabase<ThingDef>.GetNamedSilentFail(WeedDefName);

    private static readonly SimpleCurve WeedChancePerRadius =
    [
        new CurvePoint(0f, 1f),
        new CurvePoint(8f, 1f),
        new CurvePoint(11f, 0.3f)
    ];

    private static readonly SimpleCurve RadiusFactorPerPointsCurve =
    [
        new CurvePoint(25f, 0.6f),
        new CurvePoint(100f, 1f),
        new CurvePoint(500f, 2f)
    ];

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var num = RadiusFactorPerPointsCurve.Evaluate(parms.points);
        if (!TryFindRandomWeedablePlant(map, out var plant))
        {
            return false;
        }

        var room = plant.GetRoom(RegionType.Set_Passable);
        var i = 0;
        var num2 = GenRadial.NumCellsInRadius(Radius * num);
        while (i < num2)
        {
            var intVec = plant.Position + GenRadial.RadialPattern[i];
            if (intVec.InBounds(map) && intVec.GetRoom(map) == room)
            {
                var firstWeedableNowPlant = GetFirstWeedableNowPlant(intVec, map);
                if (firstWeedableNowPlant != null && firstWeedableNowPlant.def == plant.def &&
                    Rand.Chance(WeedChance(firstWeedableNowPlant.Position, plant.Position, num)))
                {
                    MakePlantWeed(firstWeedableNowPlant.Position, firstWeedableNowPlant.Map);
                }
            }

            i++;
        }

        SendStandardLetter("FE_LetterLabelWeed".Translate(new NamedArgument(plant.def, "PLANTDEF")),
            "FE_LetterWeed".Translate(new NamedArgument(plant.def, "PLANTDEF")), LetterDefOf.NegativeEvent, parms,
            new TargetInfo(plant.Position, map), []);
        return true;
    }

    public static void MakePlantWeed(IntVec3 cell, Map map)
    {
        var plant = cell.GetPlant(map);
        plant.Destroy();
        GenSpawn.Spawn(weedDef, cell, map);
    }

    private float WeedChance(IntVec3 c, IntVec3 root, float radiusFactor)
    {
        var x = c.DistanceTo(root) / radiusFactor;
        return WeedChancePerRadius.Evaluate(x);
    }

    public static Plant GetFirstWeedableNowPlant(IntVec3 c, Map map)
    {
        var plant = c.GetPlant(map);
        if (plant != null && IsWeedabe(plant))
        {
            return plant;
        }

        return null;
    }

    public static bool IsWeedabe(Plant plant)
    {
        if (plant.def.defName == WeedDefName)
        {
            return false;
        }

        if (!plant.def.plant.Blightable)
        {
            return false;
        }

        if (!plant.sown)
        {
            return false;
        }

        if (plant.LifeStage == PlantLifeStage.Sowing)
        {
            return false;
        }

        if (plant.Map.Biome.AllWildPlants.Contains(plant.def))
        {
            return false;
        }

        if (ChemicalAddedCheck.HerbicideAppliedOnto.Contains(plant))
        {
            return false;
        }

        if (ChemicalAddedCheck.DDTAppliedOnto.Contains(plant))
        {
            return false;
        }

        return !ChemicalAddedCheck.ChemicalSprayAppliedOnto.Contains(plant);
    }

    private bool TryFindRandomWeedablePlant(Map map, out Plant plant)
    {
        var result = (from x in map.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
            where IsWeedabe((Plant)x)
            select x).TryRandomElement(out var thing);
        plant = (Plant)thing;
        return result;
    }
}