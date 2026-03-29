# Fix: ResetPawnKindDefs — pawn contamination across scenarios

## What is "contamination" / Что такое "загрязнение"

**EN:** PawnKindDef contamination is when one scenario's pawn settings permanently leak into another scenario. For example, after starting the Gravship scenario (where pawns wear vacsuits), switching to Classic scenario shows all pawns still wearing vacsuits — even though Classic has nothing to do with space.

This happens because HSK's `CopyPawnKindBasics()` modifies **global** PawnKindDef objects (Colonist, Tribesperson, Survivor, etc.) instead of working on temporary copies. RimWorld stores all defs as singletons in DefDatabase — once modified, they stay modified until the game process is restarted.

The contamination affects:
- **Apparel** — vacsuits from Gravship leak into all scenarios
- **Implants** — mechlink from Mechanitor kind leaks into non-Mechanitor pawns
- **Weapons** — weapon tags/budgets can leak between scenarios

**RU:** Загрязнение PawnKindDef — это когда настройки пешек из одного сценария навсегда просачиваются в другой. Например, после запуска сценария Гравикорабль (где пешки носят скафандры), при переключении на сценарий Классика все пешки всё ещё в скафандрах — хотя Классика не имеет отношения к космосу.

Это происходит потому что `CopyPawnKindBasics()` в HSK изменяет **глобальные** объекты PawnKindDef (Colonist, Tribesperson, Survivor и т.д.) вместо работы с временными копиями. RimWorld хранит все дефы как синглтоны в DefDatabase — после изменения они остаются изменёнными до перезапуска процесса игры.

Загрязнение затрагивает:
- **Одежду** — скафандры из Гравикорабля просачиваются во все сценарии
- **Импланты** — мехлинк от Механитора просачивается к обычным пешкам
- **Оружие** — теги и бюджеты оружия могут просачиваться между сценариями

## How it happens / Как это происходит

**EN:** HSK's `SK.GeneratePawn_DefaultXenotypeRace_Patch` in Core_SK.dll randomizes PawnKindDef for extra pawns in the character selection screen. For each extra pawn it:

1. Picks a random PawnKindDef from all player-faction kinds (Colonist, Tribesperson, Mechanitor, etc.)
2. Calls `CopyPawnKindBasics(randomKind, scenarioKind)` to copy apparel/weapon settings from the scenario's kind
3. **This mutates the global def** — `randomKind` is not a copy, it's the actual DefDatabase entry

After step 3, the global `Colonist` def permanently has Gravship's vacsuit requirements. Every future use of `Colonist` (in any scenario) will generate pawns with vacsuits.

**RU:** `SK.GeneratePawn_DefaultXenotypeRace_Patch` в Core_SK.dll рандомизирует PawnKindDef для дополнительных пешек на экране выбора персонажей. Для каждой дополнительной пешки он:

1. Выбирает случайный PawnKindDef из всех игровых фракций (Colonist, Tribesperson, Mechanitor и т.д.)
2. Вызывает `CopyPawnKindBasics(randomKind, scenarioKind)` для копирования настроек одежды/оружия из сценарного вида
3. **Это мутирует глобальный деф** — `randomKind` это не копия, а реальная запись DefDatabase

После шага 3 глобальный деф `Colonist` навсегда содержит скафандровые требования Гравикорабля. Каждое будущее использование `Colonist` (в любом сценарии) будет генерировать пешек в скафандрах.

## Call Chain / Цепочка вызовов

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

## Fix / Исправление

**EN:** At game startup, snapshot all mutable fields of player-faction PawnKindDefs. Before each scenario starts (`Scenario.PreConfigure()` Harmony prefix), restore all PawnKindDefs to their original values.

**RU:** При запуске игры сохраняем снимки изменяемых полей всех PawnKindDef игровых фракций. Перед каждым запуском сценария (Harmony prefix на `Scenario.PreConfigure()`) восстанавливаем все PawnKindDef к оригинальным значениям.

### Saved/restored fields / Сохраняемые поля

| Field | Contamination type |
|---|---|
| `apparelTags` | Vacsuit tag from Gravship |
| `specificApparelRequirements` | Vacsuit body part requirements |
| `ignoreApparelAllowChance` | Bypasses generateAllowChance=0 |
| `apparelAllowHeadgearChance` | Forces helmet generation |
| `apparelMoney` | Budget changes |
| `techHediffsRequired` | Mechlink from Mechanitor |
| `techHediffsDisallowTags` | Implant restrictions |
| `techHediffsTags` | Implant tag changes |
| `techHediffsChance` | Implant chance changes |
| `techHediffsMoney` | Implant budget |
| `weaponTags` | Weapon tag changes |
| `weaponMoney` | Weapon budget |

## Limitations / Ограничения

This fix only prevents contamination **between** scenarios (cross-scenario). Contamination **within** a single scenario's pawn generation (e.g. Mechanitor losing mechlink when randomly selected and overwritten by CopyPawnKindBasics during the same batch) is not fixed — it requires Core_SK.dll to work on clones.

Этот фикс предотвращает загрязнение только **между** сценариями. Загрязнение **внутри** одной генерации пешек сценария (например Механитор теряет мехлинк при случайном выборе и перезаписи через CopyPawnKindBasics в той же партии) не исправлено — требуется работа с клонами в Core_SK.dll.

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
