using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKOdysseyFixes
{
    /// <summary>
    /// Fixes SK.GeneratePawn_DefaultXenotypeRace_Patch.CopyPawnKindBasics
    /// which doesn't reset techHediffsRequired when copying from one PawnKindDef to another.
    ///
    /// When CopyPawnKindBasics randomizes a PawnKindDef (e.g. picks Mechanitor),
    /// techHediffsRequired=[Mechlink] stays on the pawn even after copying
    /// apparelTags/Money from CrewMember. Result: CrewMember pawns spawn with a mechlink.
    ///
    /// This also affects other scenarios — any randomized PawnKindDef that happens to be
    /// Mechanitor or Mechanitor_Basic will give pawns unwanted mechlinks.
    ///
    /// See Fix_CopyPawnKindTechHediffs.README.md for full analysis.
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
            Log.Message("[HSKOdysseyFixes] Fix_CopyPawnKindTechHediffs applied.");
        }

        public static void Postfix(PawnKindDef currentPawnKind, PawnKindDef scenarioPawnKind)
        {
            // Copy techHediffsRequired from the scenario kind, replacing whatever
            // the randomized kind had (e.g. Mechanitor's [Mechlink])
            currentPawnKind.techHediffsRequired = scenarioPawnKind.techHediffsRequired;

            // Also copy techHediffsDisallowTags to maintain scenario restrictions
            currentPawnKind.techHediffsDisallowTags = scenarioPawnKind.techHediffsDisallowTags;
        }
    }
}
