# HSK Odyssey Fixes

Fixes for Odyssey DLC compatibility issues in Hardcore SK modpack.

## Fixes / Фиксы

| Fix | Type | Description |
|---|---|---|
| [ResetPawnKindDefs](Source/Fixes/ResetPawnKindDefs/Fix_ResetPawnKindDefs.README.md) | DLL | Prevents pawn contamination across scenarios (vacsuits, mechlink leak) |
| [DBH_HotSpringWash](Source/Fixes/DBH_HotSpringWash/Fix_DBH_HotSpringWash.README.md) | XML | Pawns can wash in Odyssey hot springs (DBH fix) |
| Gravship_StartingResources | XML | Unlock Hygiene I research + 25 reinforced concrete blocks |

## Fixed upstream / Исправлено в HSK ✅

These fixes were accepted into Core_SK.dll and no longer needed:
- ~~CopyPawnKindBasics~~ — vacsuits on all Gravship pawns
- ~~PowerSwitchDrawOrder~~ — cable rendering over power switch

## Known issues / Известные проблемы

Core_SK.dll `CopyPawnKindBasics()` mutates global PawnKindDef entries instead of working on clones. `ResetPawnKindDefs` restores defs before each scenario, but within a single scenario's pawn generation, contamination can still occur (e.g. mechlink leak from Mechanitor kind).

## Requirements / Требования
- RimWorld 1.5 / 1.6
- Odyssey DLC
- Hardcore SK modpack
- Harmony
- Dubs Bad Hygiene (for HotSpring fix)
