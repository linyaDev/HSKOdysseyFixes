# Fix: CopyPawnKindBasics — missing vacsuit fields

## Bug / Баг

**EN:** In TheGravship scenario, only 3 starting pawns spawn wearing vacsuits. The remaining 5 pawns in the selection pool spawn naked and die on orbital maps.

**RU:** В сценарии "Гравикорабль" только 3 стартовые пешки появляются в скафандрах. Остальные 5 пешек в пуле выбора спавнятся без одежды и погибают на орбитальных картах.

## Root Cause / Причина

**EN:** `SK.GeneratePawn_DefaultXenotypeRace_Patch` in Core_SK.dll intercepts pawn generation for `ScenPart_ConfigPage_ConfigureStartingPawns` scenarios. For extra pawns (beyond the 3 required), it picks a random `PawnKindDef` from all player-faction kinds (Colonist, Tribesperson, Survivor, etc.) and calls `CopyPawnKindBasics()` to transfer apparel settings from CrewMember.

`CopyPawnKindBasics()` copies `apparelTags`, `apparelMoney`, `weaponTags`, etc. but **misses three critical fields**:

**RU:** `SK.GeneratePawn_DefaultXenotypeRace_Patch` в Core_SK.dll перехватывает генерацию пешек для сценариев `ScenPart_ConfigPage_ConfigureStartingPawns`. Для дополнительных пешек (сверх 3 обязательных) он выбирает случайный `PawnKindDef` из всех игровых фракций (Colonist, Tribesperson, Survivor и т.д.) и вызывает `CopyPawnKindBasics()` для переноса настроек одежды из CrewMember.

`CopyPawnKindBasics()` копирует `apparelTags`, `apparelMoney`, `weaponTags` и т.д., но **пропускает три критических поля**:

| Field / Поле | Purpose / Назначение | CrewMember Value |
|---|---|---|
| `specificApparelRequirements` | Forces apparel by body part + tag / Требует одежду по части тела + тегу | Vacsuit on Torso + FullHead |
| `ignoreApparelAllowChance` | Bypasses `generateAllowChance=0` / Обходит запрет генерации | `true` |
| `apparelAllowHeadgearChance` | Headgear generation chance / Шанс генерации головного убора | `1` (always) |

## Call Chain / Цепочка вызовов

```
ScenPart_ConfigPage_ConfigureStartingPawns generates 8 pawns (3 + 5 extras)
  → PawnGenerator.GeneratePawn()
    → SK.GeneratePawn_DefaultXenotypeRace_Patch.Prefix()
      → Picks random PawnKindDef (Colonist, Survivor, etc.)
      → CopyPawnKindBasics(randomKind, CrewMember)
        ✓ copies apparelTags = [Vacsuit]
        ✓ copies apparelMoney = 2500~4000
        ✗ MISSES specificApparelRequirements
        ✗ MISSES ignoreApparelAllowChance
        ✗ MISSES apparelAllowHeadgearChance
    → PawnApparelGenerator.GenerateStartingApparelFor()
      → specificApparelRequirements is NULL → no forced vacsuit
      → ignoreApparelAllowChance is false → vacsuit filtered (generateAllowChance=0)
      → Result: pawn wearing NOTHING
```

## Fix Applied / Применённый фикс

Harmony postfix on `CopyPawnKindBasics` copies the three missing fields.

## Proposed fix for Core_SK.dll / Предложенный фикс для Core_SK.dll

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
