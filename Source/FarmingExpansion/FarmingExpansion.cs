using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;

namespace FarmingExpansion
{

    [StaticConstructorOnStartup]
	static class FarmingExpansion {


		/// <summary>
		/// This method is called on mod-startup
		/// </summary>
		static FarmingExpansion() {

			var harmony = new Harmony("Mlie.FarmingExpansion");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
