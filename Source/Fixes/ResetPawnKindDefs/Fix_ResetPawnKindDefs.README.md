# Fix: ResetPawnKindDefs — pawn contamination across scenarios

## Bug

**EN:** After starting Gravship scenario and returning to main menu, all other scenarios generate pawns wearing vacsuits. The contamination persists until the game is restarted.

**RU:** После запуска сценария Гравикорабль и возврата в главное меню, все остальные сценарии генерируют пешек в скафандрах. Загрязнение сохраняется до перезапуска игры.

## Root Cause

**EN:** `CopyPawnKindBasics()` in Core_SK.dll directly mutates PawnKindDef objects in the global DefDatabase. When Gravship scenario runs, it copies Vacsuit `specificApparelRequirements`, `ignoreApparelAllowChance`, and `apparelAllowHeadgearChance` from `CrewMember` onto randomized PawnKindDefs like `Colonist`, `Tribesperson`, `Survivor`, etc. These changes persist in memory for the entire game session because DefDatabase entries are global singletons.

**RU:** `CopyPawnKindBasics()` в Core_SK.dll напрямую мутирует объекты PawnKindDef в глобальной DefDatabase. Когда запускается сценарий Гравикорабль, он копирует скафандровые поля (`specificApparelRequirements`, `ignoreApparelAllowChance`, `apparelAllowHeadgearChance`) из `CrewMember` в рандомизированные PawnKindDef — `Colonist`, `Tribesperson`, `Survivor` и т.д. Эти изменения сохраняются в памяти на всю сессию, потому что записи DefDatabase — глобальные синглтоны.

## Call Chain

```
Start Gravship scenario
  → ScenPart_ConfigPage_ConfigureStartingPawns generates 8 pawns
    → SK.GeneratePawn_DefaultXenotypeRace_Patch picks random PawnKindDef
    → CopyPawnKindBasics(Colonist, CrewMember)
      → Colonist.specificApparelRequirements = [Vacsuit on Torso, Vacsuit on FullHead]
      → Colonist.ignoreApparelAllowChance = true
      → Colonist.apparelAllowHeadgearChance = 1
      → (mutates global Colonist def permanently)

Return to main menu, start Classic scenario
  → Colonist def still has Vacsuit requirements from previous scenario
  → All Classic pawns spawn wearing vacsuits
```

## Fix

**EN:** At game startup, snapshot all mutable fields of player-faction PawnKindDefs. Before each scenario starts (`Scenario.PreConfigure()` Harmony prefix), restore all PawnKindDefs to their original values.

**RU:** При запуске игры сохраняем снимки изменяемых полей всех PawnKindDef игровых фракций. Перед каждым запуском сценария (Harmony prefix на `Scenario.PreConfigure()`) восстанавливаем все PawnKindDef к оригинальным значениям.

### Saved/restored fields

| Field | Why |
|---|---|
| `apparelTags` | Vacsuit tag contamination |
| `specificApparelRequirements` | Vacsuit body part requirements |
| `ignoreApparelAllowChance` | Bypasses generateAllowChance=0 |
| `apparelAllowHeadgearChance` | Forces helmet generation |
| `apparelMoney` | Budget changes |
| `techHediffsRequired` | Mechlink contamination |
| `techHediffsDisallowTags` | Implant restrictions |
| `techHediffsTags` | Implant tag changes |
| `techHediffsChance` | Implant chance changes |
| `techHediffsMoney` | Implant budget |
| `weaponTags` | Weapon tag changes |
| `weaponMoney` | Weapon budget |

## Proposed fix for Core_SK.dll (Not tested)

`CopyPawnKindBasics()` should work on a **copy** of the PawnKindDef instead of mutating the original:

```csharp
// Instead of:
CopyPawnKindBasics(pawnkind, request.KindDef);
// Use cloned PawnKindDef:
PawnKindDef clone = ClonePawnKindDef(pawnkind);
CopyPawnKindBasics(clone, request.KindDef);
request.KindDef = clone;
```
