using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKOdysseyFixes
{
    /// <summary>
    /// Fixes SK.GeneratePawn_DefaultXenotypeRace_Patch.CopyPawnKindBasics
    /// which doesn't copy these fields from the scenario PawnKindDef:
    ///
    ///   - specificApparelRequirements  (forces Vacsuit tag on Torso + FullHead)
    ///   - ignoreApparelAllowChance     (bypasses generateAllowChance=0 on vacsuit ThingDef)
    ///   - apparelAllowHeadgearChance   (ensures VacsuitHelmet is always generated)
    ///
    /// Without this fix, only 3 starting CrewMember pawns get vacsuits in TheGravship scenario.
    /// The remaining pawns get randomized to Colonist, Tribesperson, Survivor, etc.
    /// via CopyPawnKindBasics, which copies apparelTags and apparelMoney but not the
    /// fields above — so PawnApparelGenerator cannot generate vacsuit apparel for them.
    ///
    /// See Fix_CopyPawnKindBasics.README.md for full analysis and proposed Core_SK.dll fix.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Fix_CopyPawnKindBasics
    {
        static Fix_CopyPawnKindBasics()
        {
            var harmony = new Harmony("linya.hskodysseyfixes.copypawnkindbasics");

            var skType = AccessTools.TypeByName("SK.GeneratePawn_DefaultXenotypeRace_Patch");
            if (skType == null) return;

            var original = AccessTools.Method(skType, "CopyPawnKindBasics");
            if (original == null) return;

            harmony.Patch(original, postfix: new HarmonyMethod(typeof(Fix_CopyPawnKindBasics), nameof(Postfix)));
            Log.Message("[HSKOdysseyFixes] Fix_CopyPawnKindBasics applied.");
        }

        public static void Postfix(PawnKindDef currentPawnKind, PawnKindDef scenarioPawnKind)
        {
            if (scenarioPawnKind.specificApparelRequirements != null)
            {
                currentPawnKind.specificApparelRequirements = scenarioPawnKind.specificApparelRequirements;
            }

            if (scenarioPawnKind.ignoreApparelAllowChance)
            {
                currentPawnKind.ignoreApparelAllowChance = true;
            }

            currentPawnKind.apparelAllowHeadgearChance = scenarioPawnKind.apparelAllowHeadgearChance;
        }
    }
}
