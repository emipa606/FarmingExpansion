using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace FarmingExpansion
{
    internal class WeedDef : Plant
    {
        public override void TickLong()
        {
            base.TickLong();
            if (Rand.Range(-1, 5) < 0)
            {
                TryReproduceNow();
            }
        }

        public void TryReproduceNow()
        {
            GenRadial.ProcessEquidistantCells(Position, 4f, delegate(List<IntVec3> cells)
            {
                if (!(from x in cells
                    where IncidentWorker_Weeds.GetFirstWeedableNowPlant(x, Map) != null
                    select x).TryRandomElement(out var c))
                {
                    return false;
                }

                IncidentWorker_Weeds.MakePlantWeed(c, Map);
                return true;
            }, Map);
        }
    }
}