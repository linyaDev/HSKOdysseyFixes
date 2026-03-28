# HSK Odyssey Fixes

Fixes for Odyssey DLC compatibility issues in Hardcore SK modpack.
Each fix has its own README in `Source/Fixes/` with detailed description.

## Fixes / Фиксы

| Fix | Type | Description |
|---|---|---|
| [CopyPawnKindBasics](Source/Fixes/CopyPawnKindBasics/Fix_CopyPawnKindBasics.README.md) | DLL | Gravship scenario: all pawns spawn wearing vacsuits |
| [CopyPawnKindTechHediffs](Source/Fixes/CopyPawnKindTechHediffs/Fix_CopyPawnKindTechHediffs.README.md) | DLL | Prevents random mechlink on scenario pawns |
| [DBH_HotSpringWash](Source/Fixes/DBH_HotSpringWash/Fix_DBH_HotSpringWash.README.md) | DLL | Pawns can wash in Odyssey hot springs (DBH fix) |

## Structure / Структура

```
HSKOdysseyFixes/
├── About/About.xml
├── Assemblies/
│   ├── Fix_CopyPawnKindBasics.dll
│   ├── Fix_CopyPawnKindTechHediffs.dll
│   └── Fix_DBH_HotSpringWash.dll
├── Source/Fixes/
│   ├── CopyPawnKindBasics/
│   ├── CopyPawnKindTechHediffs/
│   └── DBH_HotSpringWash/
└── README.md
```

## Requirements / Требования
- RimWorld 1.5 / 1.6
- Odyssey DLC
- Hardcore SK modpack
- Harmony
- Dubs Bad Hygiene (for HotSpring fix)
