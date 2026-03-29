using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKOdysseyFixes
{
    /// <summary>
    /// Fixes SK.GeneratePawn_DefaultXenotypeRace_Patch.CopyPawnKindBasics
    /// which only copies techHediffsRequired when source is non-null.
    ///
    /// When Mechanitor PawnKindDef is randomly selected (techHediffsRequired=[Mechlink])
    /// and the scenario kind (CrewMember) has techHediffsRequired=null,
    /// the conditional copy doesn't execute, leaving Mechlink on the pawn.
    ///
    /// Fix: unconditionally assign techHediffsRequired from scenario kind,
    /// which resets it to null when the scenario kind has no required hediffs.
    ///
    /// Core_SK.dll bug (still present after ad107c4a):
    ///   if (scenarioPawnKind.techHediffsRequired != null)  // ← wrong: skips null reset
    ///       currentPawnKind.techHediffsRequired = scenarioPawnKind.techHediffsRequired;
    ///
    /// Should be:
    ///   currentPawnKind.techHediffsRequired = scenarioPawnKind.techHediffsRequired;
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Fix_CopyPawnKindTechHediffs
    {
        static Fix_CopyPawnKindTechHediffs()
        {
            var harmony = new Harmony("linya.hskodysseyfixes.copypawnkindtechhediffs");

            var skType = AccessTools.TypeByName("SK.GeneratePawn_DefaultXenotypeRace_Patch");
            if (skType == null) return;

            var original = AccessTools.Method(skType, "CopyPawnKindBasics");
            if (original == null) return;

            harmony.Patch(original, postfix: new HarmonyMethod(typeof(Fix_CopyPawnKindTechHediffs), nameof(Postfix)));
            Log.Message("[HSKOdysseyFixes] Fix_CopyPawnKindTechHediffs applied — unconditional techHediffsRequired reset.");
        }

        public static void Postfix(PawnKindDef currentPawnKind, PawnKindDef scenarioPawnKind)
        {
            // Unconditionally assign — if scenario kind has null, reset to null
            currentPawnKind.techHediffsRequired = scenarioPawnKind.techHediffsRequired;
            currentPawnKind.techHediffsDisallowTags = scenarioPawnKind.techHediffsDisallowTags;
        }
    }
}
