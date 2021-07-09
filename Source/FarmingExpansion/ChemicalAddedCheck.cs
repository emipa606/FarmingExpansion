using System.Collections.Generic;
using System.Threading;
using RimWorld;
using Verse;

namespace FarmingExpansion
{
    /// <summary>
    ///     Checks every minute (3600 ticks) if all of the chemical-applied plants still exists. If not, removes the plant's
    ///     entry in the Dictionary
    ///     containing all chemical-applied plants so that chemicals can be applied onto a new plant on that cell on that map
    /// </summary>
    internal class ChemicalAddedCheck : GameComponent
    {
        public static List<Plant> FungicideAppliedOnto;
        public static List<Plant> HerbicideAppliedOnto;
        public static List<Plant> PesticideAppliedOnto;
        public static List<Plant> DDTAppliedOnto;
        public static List<Plant> ChemicalSprayAppliedOnto;

        private int FunctionWasLastRunOnTick;

        /// <summary>
        ///     Empty constructor with no function other than to prevent an error from when the vanilla game tries to access the
        ///     constructor
        /// </summary>
        public ChemicalAddedCheck(Game game)
        {
        }

        public override void StartedNewGame()
        {
            FungicideAppliedOnto = new List<Plant>();
            HerbicideAppliedOnto = new List<Plant>();
            PesticideAppliedOnto = new List<Plant>();
            DDTAppliedOnto = new List<Plant>();
            ChemicalSprayAppliedOnto = new List<Plant>();
        }

        /// <summary>
        ///     Checks whether all plants that chemicals have been applied onto still exists. If not, removes that plant's entry in
        ///     the dictionary
        /// </summary>
        public override void GameComponentTick()
        {
            // Checks the dictionary every 3600 ticks (every minute)
            if (FunctionWasLastRunOnTick + 3600 > Current.Game.tickManager.TicksGame)
            {
                return;
            }

            // The check is done in another thread, because the check takes so long time that the game becomes out of sync if done in the main thread
            var t = new Thread(() =>
            {
                foreach (var list in new List<List<Plant>>
                {
                    FungicideAppliedOnto, HerbicideAppliedOnto, PesticideAppliedOnto, DDTAppliedOnto,
                    ChemicalSprayAppliedOnto
                })
                {
                    foreach (var plant in list)
                    {
                        // If the same plant is not on the cell, this map does contain that plant
                        if (plant == null || plant != plant.Position.GetPlant(plant.Map))
                        {
                            list.Remove(plant);
                        }
                    }
                }
            });
            t.Start();
            FunctionWasLastRunOnTick = Current.Game.tickManager.TicksGame;
        }

        public override void ExposeData()
        {
            // This saves the list
            Scribe_Collections.Look(ref FungicideAppliedOnto, "FungicideAppliedOnto", LookMode.Reference);
            Scribe_Collections.Look(ref HerbicideAppliedOnto, "HerbicideAppliedOnto", LookMode.Reference);
            Scribe_Collections.Look(ref PesticideAppliedOnto, "PesticideAppliedOnto", LookMode.Reference);
            Scribe_Collections.Look(ref DDTAppliedOnto, "DDTAppliedOnto", LookMode.Reference);
            Scribe_Collections.Look(ref ChemicalSprayAppliedOnto, "ChemicalSprayAppliedOnto", LookMode.Reference);
            Scribe_Collections.Look(ref UseChemicalWorkGiver.IsGrowingZonesWithChemicalList,
                "IsGrowingZonesWithChemicalList", LookMode.Reference);
        }
    }
}