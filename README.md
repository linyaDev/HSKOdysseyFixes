# HSK Odyssey Fixes

Fixes for Odyssey DLC compatibility issues in Hardcore SK modpack.

## Fixes / Фиксы

| Fix | Type | Description |
|---|---|---|
| CopyPawnKindTechHediffs | DLL | Prevents random mechlink (Core_SK.dll bug: conditional instead of unconditional reset) |
| [DBH_HotSpringWash](Source/Fixes/DBH_HotSpringWash/Fix_DBH_HotSpringWash.README.md) | XML+DLL | Pawns can wash in Odyssey hot springs (DBH fix) |
| Gravship_StartingResources | XML | Unlock Hygiene I research + 25 reinforced concrete blocks |

## Fixed upstream / Исправлено в HSK ✅

These fixes were accepted into Core_SK.dll and no longer needed:
- ~~CopyPawnKindBasics~~ — vacsuits on all Gravship pawns
- ~~CopyPawnKindTechHediffs~~ — partially fixed, but uses conditional `if (!=null)` instead of unconditional assign, so mechlink still not reset when scenario kind has null techHediffsRequired
- ~~PowerSwitchDrawOrder~~ — cable rendering over power switch

## Requirements / Требования
- RimWorld 1.5 / 1.6
- Odyssey DLC
- Hardcore SK modpack
- Harmony
- Dubs Bad Hygiene (for HotSpring fix)
