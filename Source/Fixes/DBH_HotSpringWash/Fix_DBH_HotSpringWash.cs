using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace HSKOdysseyFixes
{
    /// <summary>
    /// Fixes Dubs Bad Hygiene not recognizing Odyssey's HotSpring terrain as water.
    ///
    /// DBH's DefExtensions.ResolveSurfaceWater() adds tag "dbh_water" to vanilla
    /// water terrains (WaterShallow, WaterDeep, etc.) but doesn't know about
    /// Odyssey's HotSpring TerrainDef. Without this tag, pawns won't wash in
    /// hot springs even though they are clean water.
    ///
    /// Fix: after ResolveSurfaceWater() runs, add "dbh_water" tag to HotSpring terrain.
    ///
    /// See Fix_DBH_HotSpringWash.README.md for full analysis.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Fix_DBH_HotSpringWash
    {
        static Fix_DBH_HotSpringWash()
        {
            var harmony = new Harmony("linya.hskodysseyfixes.dbh.hotspringwash");

            var dbhType = AccessTools.TypeByName("DubsBadHygiene.DefExtensions");
            if (dbhType == null)
            {
                Log.Message("[HSKOdysseyFixes] DBH not found, skipping HotSpring fix.");
                return;
            }

            var original = AccessTools.Method(dbhType, "ResolveSurfaceWater");
            if (original == null)
            {
                Log.Warning("[HSKOdysseyFixes] ResolveSurfaceWater not found in DBH.");
                return;
            }

            harmony.Patch(original,
                postfix: new HarmonyMethod(typeof(Fix_DBH_HotSpringWash), nameof(Postfix)));
            Log.Message("[HSKOdysseyFixes] Fix_DBH_HotSpringWash applied.");
        }

        public static void Postfix()
        {
            var hotSpring = DefDatabase<TerrainDef>.GetNamedSilentFail("HotSpring");
            if (hotSpring == null) return;

            if (hotSpring.tags == null)
            {
                hotSpring.tags = new List<string>();
            }

            if (!hotSpring.tags.Contains("dbh_water"))
            {
                hotSpring.tags.Add("dbh_water");
                Log.Message("[HSKOdysseyFixes] Added dbh_water tag to HotSpring terrain.");
            }
        }
    }
}
