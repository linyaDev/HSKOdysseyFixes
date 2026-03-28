using HarmonyLib;
using RimWorld;
using Verse;

namespace HSKOdysseyFixes
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("linya.hskodysseyfixes");

            // Patch SK.GeneratePawn_DefaultXenotypeRace_Patch.CopyPawnKindBasics
            // This method copies apparel/weapon settings when HSK randomizes PawnKindDef
            // for extra pawns in the character selection pool, but it misses three fields
            // critical for vacsuit generation in TheGravship scenario.
            var skType = AccessTools.TypeByName("SK.GeneratePawn_DefaultXenotypeRace_Patch");
            if (skType != null)
            {
                var original = AccessTools.Method(skType, "CopyPawnKindBasics");
                if (original != null)
                {
                    harmony.Patch(original,
                        postfix: new HarmonyMethod(typeof(Patch_CopyPawnKindBasics),
                            nameof(Patch_CopyPawnKindBasics.Postfix)));
                    Log.Message("[HSKOdysseyFixes] Patched CopyPawnKindBasics — vacsuit fields will be copied.");
                }
            }
        }
    }

    /// <summary>
    /// Fixes SK.GeneratePawn_DefaultXenotypeRace_Patch.CopyPawnKindBasics
    /// which doesn't copy these fields from the scenario PawnKindDef:
    ///
    ///   - specificApparelRequirements  (forces Vacsuit tag on Torso + FullHead)
    ///   - ignoreApparelAllowChance     (bypasses generateAllowChance=0 on vacsuit ThingDef)
    ///   - apparelAllowHeadgearChance   (ensures VacsuitHelmet is always generated)
    ///
    /// Without this fix, only 3 starting CrewMember pawns get vacsuits.
    /// The remaining pawns in the selection pool get randomized to Colonist,
    /// Tribesperson, Survivor, etc. via CopyPawnKindBasics, which copies
    /// apparelTags and apparelMoney but not the fields above — so PawnApparelGenerator
    /// cannot find valid vacsuit apparel for them.
    ///
    /// ===== FIX FOR Core_SK.dll =====
    ///
    /// In Core_SK.dll, class SK.GeneratePawn_DefaultXenotypeRace_Patch,
    /// method CopyPawnKindBasics(PawnKindDef currentPawnKind, PawnKindDef scenarioPawnKind),
    /// add these lines before the return statement:
    ///
    ///   if (scenarioPawnKind.specificApparelRequirements != null)
    ///   {
    ///       currentPawnKind.specificApparelRequirements = scenarioPawnKind.specificApparelRequirements;
    ///   }
    ///   if (scenarioPawnKind.ignoreApparelAllowChance)
    ///   {
    ///       currentPawnKind.ignoreApparelAllowChance = true;
    ///   }
    ///   currentPawnKind.apparelAllowHeadgearChance = scenarioPawnKind.apparelAllowHeadgearChance;
    ///
    /// </summary>
    public static class Patch_CopyPawnKindBasics
    {
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
