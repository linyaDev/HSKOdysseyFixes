# Odyssey DLC Blockers / Блокеры DLC Odyssey

Issues that prevent or significantly hinder playing Odyssey DLC with Hardcore SK.
Проблемы, которые мешают или значительно затрудняют игру в DLC Odyssey с Hardcore SK.

## Fixed / Исправлено ✅

### 1. Gravship scenario — pawns without vacsuits / пешки без скафандров
- **Status:** Fixed (Fix_CopyPawnKindBasics.dll)
- **Problem:** Only 3 starting pawns spawn wearing vacsuits. Extra pawns in selection pool spawn naked.
- **Cause:** `CopyPawnKindBasics()` in Core_SK.dll doesn't copy `specificApparelRequirements`, `ignoreApparelAllowChance`, `apparelAllowHeadgearChance`.
- **Проблема:** Только 3 стартовые пешки в скафандрах. Остальные в пуле выбора без одежды.

### 2. Gravship scenario — random mechlink / случайный мехлинк
- **Status:** Fixed (Fix_CopyPawnKindTechHediffs.dll)
- **Problem:** Some pawns spawn with mechlink implant when Mechanitor PawnKindDef is randomly selected.
- **Cause:** `CopyPawnKindBasics()` doesn't reset `techHediffsRequired`.
- **Проблема:** Некоторые пешки спавнятся с мехлинком из-за случайного выбора PawnKindDef Mechanitor.

### 3. Gravship scenario — no ammo for Uzi / нет патронов для Uzi
- **Status:** Fixed (PR to HSK: linyaDev/Hardcore-SK@aaa512c8)
- **Problem:** Uzi uses 9x19mm Para in HSK but only .45 ACP ammo was provided.
- **Cause:** CE patch sets .45 ACP, but HSK overrides Gun_MachinePistol to 9x19mm.
- **Проблема:** Uzi использует 9x19mm Para в HSK, но в сценарии были только патроны .45 ACP.

### 4. DBH — can't wash in hot springs / нельзя мыться в горячих источниках
- **Status:** Fixed (Patch_DBH_HotSpringWash.xml + Fix_DBH_HotSpringWash.dll)
- **Problem:** Pawns refuse to wash in Odyssey hot springs.
- **Cause:** DBH `ResolveSurfaceWater()` doesn't add `dbh_water` tag to HotSpring TerrainDef.
- **Проблема:** Пешки отказываются мыться в горячих источниках Odyssey.

## Open / Открытые 🔴

### 5. Chem refinery requires reinforced concrete blocks / Переработчик химии требует железобетонные блоки
- **Status:** Open
- **Severity:** Critical
- **Problem:** Chem refinery (chemfuel refinery) requires 25 reinforced concrete blocks to build. On orbital/gravship maps there is no way to obtain reinforced concrete — no research, no resources, no traders.
- **Проблема:** Переработчик химии требует 25 железобетонных блоков на постройку. На орбитальных/гравикорабельных картах железобетон неоткуда взять — нет исследований, ресурсов, торговцев.
- **Proposed solutions / Предложения:**
  1. Add 25 reinforced concrete blocks to Gravship scenario starting items / Дать 25 железобетонных блоков на старте в сценарии
  2. Simplify the refinery recipe — remove reinforced concrete requirement / Упростить рецепт — убрать требование железобетона
  3. Create a new simpler refinery variant for orbital use / Сделать новый упрощённый перегонный аппарат для орбиты

### 6. Steam generator requires water / Паровой генератор требует воду
- **Status:** Open
- **Severity:** Critical
- **Problem:** Steam generator requires water connection (DBH water system) which doesn't work on gravship/orbital maps. No water infrastructure available in space.
- **Проблема:** Паровой генератор требует подключение воды (система воды DBH), которая не работает на картах гравикорабля/орбиты. В космосе нет водной инфраструктуры.

<!--
Template / Шаблон:

### N. Short description / Краткое описание
- **Status:** Open / Investigating / Fixed
- **Severity:** Critical / Major / Minor
- **Problem:** What happens
- **Expected:** What should happen
- **Cause:** Root cause (if known)
- **Проблема:** Описание на русском
-->
