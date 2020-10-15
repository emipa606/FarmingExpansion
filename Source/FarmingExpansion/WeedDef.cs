using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FarmingExpansion
{
    class WeedDef : Plant
    {
        public override void TickLong()
        {
            base.TickLong();
            if (Rand.Range(-1, 5) < 0)
                TryReproduceNow();
        }

        public void TryReproduceNow()
        {
            GenRadial.ProcessEquidistantCells(base.Position, 4f, delegate (List<IntVec3> cells)
            {
                if ((from x in cells
                     where IncidentWorker_Weeds.GetFirstWeedableNowPlant(x, base.Map) != null
                     select x).TryRandomElement(out IntVec3 c))
                {
                    IncidentWorker_Weeds.MakePlantWeed(c, base.Map);
                    return true;
                }
                return false;
            }, base.Map);
        }
    }
}
