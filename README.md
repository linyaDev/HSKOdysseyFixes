# HSK Odyssey Fixes

Fixes Odyssey DLC compatibility issues in Hardcore SK modpack.

## Bug: Gravship scenario — pawns spawn without vacsuits

In TheGravship scenario, only 3 starting pawns (kindDef=CrewMember) spawn wearing vacsuits. The remaining 5 pawns in the selection pool spawn naked and die on orbital maps.

## Root Cause

`SK.GeneratePawn_DefaultXenotypeRace_Patch` in **Core_SK.dll** intercepts pawn generation for `ScenPart_ConfigPage_ConfigureStartingPawns` scenarios. For extra pawns (beyond the 3 required), it:

1. Picks a random `PawnKindDef` from all player-faction kinds (Colonist, Tribesperson, Survivor, NuclearSurvivor, etc.)
2. Calls `CopyPawnKindBasics()` to transfer apparel/weapon settings from the original kindDef (CrewMember) to the randomized one

**The problem:** `CopyPawnKindBasics()` copies `apparelTags`, `apparelMoney`, `weaponTags`, etc. but **misses three critical fields**:

| Missing Field | Purpose | CrewMember Value |
|---|---|---|
| `specificApparelRequirements` | Forces specific apparel by body part + tag | Vacsuit on Torso + FullHead |
| `ignoreApparelAllowChance` | Bypasses `generateAllowChance=0` on ThingDef | `true` (vacsuit has generateAllowChance=0) |
| `apparelAllowHeadgearChance` | Probability of generating headgear | `1` (always generate VacsuitHelmet) |

Without these fields, `PawnApparelGenerator` cannot find valid vacsuit apparel for the randomized PawnKindDef, so pawns spawn naked.

## Fix

This mod patches `CopyPawnKindBasics` via Harmony postfix to copy the missing fields.

### Proposed fix for Core_SK.dll

In `SK.GeneratePawn_DefaultXenotypeRace_Patch.CopyPawnKindBasics()`, add before `return currentPawnKind;`:

```csharp
if (scenarioPawnKind.specificApparelRequirements != null)
{
    currentPawnKind.specificApparelRequirements = scenarioPawnKind.specificApparelRequirements;
}
if (scenarioPawnKind.ignoreApparelAllowChance)
{
    currentPawnKind.ignoreApparelAllowChance = true;
}
currentPawnKind.apparelAllowHeadgearChance = scenarioPawnKind.apparelAllowHeadgearChance;
```

## Call Chain

```
Player starts TheGravship scenario
  → ScenPart_ConfigPage_ConfigureStartingPawns generates 8 pawns (3 required + 5 extras)
    → PawnGenerator.GeneratePawn() called for each
      → SK.GeneratePawn_DefaultXenotypeRace_Patch.Prefix() intercepts
        → For ScenPart_ConfigPage_ConfigureStartingPawns:
          → Picks random PawnKindDef from DefDatabase (Colonist, Survivor, etc.)
          → CopyPawnKindBasics(randomKind, CrewMember) ← MISSES 3 FIELDS
        → PawnApparelGenerator.GenerateStartingApparelFor()
          → specificApparelRequirements is NULL → no forced vacsuit
          → ignoreApparelAllowChance is false → vacsuit filtered out (generateAllowChance=0)
          → apparelAllowHeadgearChance defaults → helmet may not generate
        → Result: pawn wearing NOTHING
```

## Requirements
- RimWorld 1.5 / 1.6
- Odyssey DLC
- Hardcore SK modpack
- Harmony
