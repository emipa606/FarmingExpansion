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
        var harmony = new Harmony("Mlie.FarmingExpansion");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}