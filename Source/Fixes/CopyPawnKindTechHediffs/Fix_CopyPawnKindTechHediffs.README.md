# Fix: CopyPawnKindBasics — unwanted techHediffsRequired (mechlink)

## Bug / Баг

**EN:** In scenarios using `ScenPart_ConfigPage_ConfigureStartingPawns` (Gravship, Classic, etc.), some pawns randomly spawn with a mechlink implant even though the scenario doesn't require it.

**RU:** В сценариях, использующих `ScenPart_ConfigPage_ConfigureStartingPawns` (Гравикорабль, Классика и т.д.), некоторые пешки случайно спавнятся с мехлинком, хотя сценарий этого не требует.

## Root Cause / Причина

**EN:** `SK.GeneratePawn_DefaultXenotypeRace_Patch` in Core_SK.dll randomizes PawnKindDef for extra pawns from all player-faction kinds. The vanilla `Mechanitor` PawnKindDef has:
- `defaultFactionDef: PlayerColony` (`isPlayer: true`) — so it's in the random pool
- `techHediffsRequired: [Mechlink]` — forces mechlink implant

When `CopyPawnKindBasics()` is called, it copies `techHediffsTags`, `techHediffsChance`, and `techHediffsMoney` from the scenario kind (e.g. CrewMember), but **does not reset `techHediffsRequired`**. So if the randomized kind was `Mechanitor`, the pawn keeps `techHediffsRequired: [Mechlink]` and spawns with a mechlink.

**RU:** `SK.GeneratePawn_DefaultXenotypeRace_Patch` в Core_SK.dll рандомизирует PawnKindDef для дополнительных пешек из всех игровых фракций. Ванильный `Mechanitor` имеет:
- `defaultFactionDef: PlayerColony` (`isPlayer: true`) — попадает в пул рандомизации
- `techHediffsRequired: [Mechlink]` — принудительно устанавливает мехлинк

Когда вызывается `CopyPawnKindBasics()`, метод копирует `techHediffsTags`, `techHediffsChance` и `techHediffsMoney` из сценарного kindDef (например CrewMember), но **не сбрасывает `techHediffsRequired`**. Если рандом выбрал `Mechanitor`, пешка сохраняет `techHediffsRequired: [Mechlink]` и спавнится с мехлинком.

## Affected PawnKindDefs / Затронутые PawnKindDef

| PawnKindDef | techHediffsRequired | Source |
|---|---|---|
| `Mechanitor` | `[Mechlink]` | Biotech DLC |
| `Mechanitor_Basic` | (inherited from Mechanitor) | Biotech DLC |

## Call Chain / Цепочка вызовов

```
ScenPart_ConfigPage_ConfigureStartingPawns generates extra pawns
  → SK.GeneratePawn_DefaultXenotypeRace_Patch.Prefix()
    → Picks random PawnKindDef from all where defaultFactionDef.isPlayer
    → Random picks Mechanitor (has techHediffsRequired: [Mechlink])
    → CopyPawnKindBasics(Mechanitor, CrewMember)
      ✓ copies techHediffsTags from CrewMember
      ✓ copies techHediffsMoney from CrewMember
      ✓ copies techHediffsChance from CrewMember
      ✗ DOES NOT reset techHediffsRequired (stays [Mechlink])
    → PawnGenerator adds Mechlink to pawn
    → Result: CrewMember pawn with unwanted mechlink
```

## Fix Applied / Применённый фикс

Harmony postfix on `CopyPawnKindBasics` copies `techHediffsRequired` and `techHediffsDisallowTags` from the scenario PawnKindDef, replacing whatever the randomized kind had.

## Proposed fix for Core_SK.dll / Предложенный фикс для Core_SK.dll

In `SK.GeneratePawn_DefaultXenotypeRace_Patch.CopyPawnKindBasics()`, add before `return currentPawnKind;`:

```csharp
currentPawnKind.techHediffsRequired = scenarioPawnKind.techHediffsRequired;
currentPawnKind.techHediffsDisallowTags = scenarioPawnKind.techHediffsDisallowTags;
```
