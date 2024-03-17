using System.Reflection;
using HarmonyLib;
using Verse;

namespace FarmingExpansion;

[StaticConstructorOnStartup]
internal static class FarmingExpansion
{
    /// <summary>
    ///     This method is called on mod-startup
    /// </summary>
    static FarmingExpansion()
    {
        new Harmony("Mlie.FarmingExpansion").PatchAll(Assembly.GetExecutingAssembly());
    }
}