using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKOdysseyFixes
{
    /// <summary>
    /// Fixes pawn contamination across scenarios.
    ///
    /// CopyPawnKindBasics in Core_SK.dll mutates global PawnKindDef entries
    /// in DefDatabase. When Gravship scenario copies Vacsuit requirements
    /// onto Colonist def, that change persists for all subsequent scenarios
    /// in the same game session.
    ///
    /// Fix: on game startup, snapshot mutable fields of all player PawnKindDefs.
    /// Before each scenario starts (Scenario.PreConfigure), restore snapshots.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Fix_ResetPawnKindDefs
    {
        private struct PawnKindSnapshot
        {
            public List<string> apparelTags;
            public List<SpecificApparelRequirement> specificApparelRequirements;
            public bool ignoreApparelAllowChance;
            public float apparelAllowHeadgearChance;
            public FloatRange apparelMoney;
            public List<ThingDef> techHediffsRequired;
            public List<string> techHediffsDisallowTags;
            public List<string> techHediffsTags;
            public float techHediffsChance;
            public FloatRange techHediffsMoney;
            public List<string> weaponTags;
            public FloatRange weaponMoney;
        }

        private static Dictionary<PawnKindDef, PawnKindSnapshot> snapshots = new Dictionary<PawnKindDef, PawnKindSnapshot>();

        static Fix_ResetPawnKindDefs()
        {
            // Save snapshots of all player-faction PawnKindDefs
            foreach (var kind in DefDatabase<PawnKindDef>.AllDefsListForReading)
            {
                if (kind.defaultFactionDef != null && kind.defaultFactionDef.isPlayer)
                {
                    snapshots[kind] = new PawnKindSnapshot
                    {
                        apparelTags = kind.apparelTags != null ? new List<string>(kind.apparelTags) : null,
                        specificApparelRequirements = kind.specificApparelRequirements,
                        ignoreApparelAllowChance = kind.ignoreApparelAllowChance,
                        apparelAllowHeadgearChance = kind.apparelAllowHeadgearChance,
                        apparelMoney = kind.apparelMoney,
                        techHediffsRequired = kind.techHediffsRequired != null ? new List<ThingDef>(kind.techHediffsRequired) : null,
                        techHediffsDisallowTags = kind.techHediffsDisallowTags != null ? new List<string>(kind.techHediffsDisallowTags) : null,
                        techHediffsTags = kind.techHediffsTags != null ? new List<string>(kind.techHediffsTags) : null,
                        techHediffsChance = kind.techHediffsChance,
                        techHediffsMoney = kind.techHediffsMoney,
                        weaponTags = kind.weaponTags != null ? new List<string>(kind.weaponTags) : null,
                        weaponMoney = kind.weaponMoney,
                    };
                }
            }

            var harmony = new Harmony("linya.hskodysseyfixes.resetpawnkinddefs");
            harmony.Patch(
                AccessTools.Method(typeof(Scenario), "PreConfigure"),
                prefix: new HarmonyMethod(typeof(Fix_ResetPawnKindDefs), nameof(Prefix)));

            Log.Message($"[HSKOdysseyFixes] Fix_ResetPawnKindDefs: saved {snapshots.Count} PawnKindDef snapshots.");
        }

        public static void Prefix()
        {
            int restored = 0;
            foreach (var kvp in snapshots)
            {
                var kind = kvp.Key;
                var snap = kvp.Value;

                kind.apparelTags = snap.apparelTags != null ? new List<string>(snap.apparelTags) : null;
                kind.specificApparelRequirements = snap.specificApparelRequirements;
                kind.ignoreApparelAllowChance = snap.ignoreApparelAllowChance;
                kind.apparelAllowHeadgearChance = snap.apparelAllowHeadgearChance;
                kind.apparelMoney = snap.apparelMoney;
                kind.techHediffsRequired = snap.techHediffsRequired != null ? new List<ThingDef>(snap.techHediffsRequired) : null;
                kind.techHediffsDisallowTags = snap.techHediffsDisallowTags != null ? new List<string>(snap.techHediffsDisallowTags) : null;
                kind.techHediffsTags = snap.techHediffsTags != null ? new List<string>(snap.techHediffsTags) : null;
                kind.techHediffsChance = snap.techHediffsChance;
                kind.techHediffsMoney = snap.techHediffsMoney;
                kind.weaponTags = snap.weaponTags != null ? new List<string>(snap.weaponTags) : null;
                kind.weaponMoney = snap.weaponMoney;

                restored++;
            }
            Log.Message($"[HSKOdysseyFixes] Fix_ResetPawnKindDefs: restored {restored} PawnKindDefs before scenario start.");
        }
    }
}
