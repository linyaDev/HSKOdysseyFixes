# HSK Odyssey Fixes

Fixes for Odyssey DLC compatibility issues in Hardcore SK modpack.
Each fix has its own README in `Source/Fixes/` with detailed description.

## Fixes / Фиксы

| Fix | Type | Description |
|---|---|---|
| [CopyPawnKindBasics](Source/Fixes/CopyPawnKindBasics/Fix_CopyPawnKindBasics.README.md) | DLL | Gravship scenario: all pawns spawn wearing vacsuits |
| [CopyPawnKindTechHediffs](Source/Fixes/CopyPawnKindTechHediffs/Fix_CopyPawnKindTechHediffs.README.md) | DLL | Prevents random mechlink on scenario pawns |

## Structure / Структура

```
HSKOdysseyFixes/
├── About/About.xml
├── Assemblies/
│   ├── Fix_CopyPawnKindBasics.dll
│   └── Fix_CopyPawnKindTechHediffs.dll
├── Source/Fixes/
│   ├── CopyPawnKindBasics/
│   │   ├── Fix_CopyPawnKindBasics.cs
│   │   ├── Fix_CopyPawnKindBasics.csproj
│   │   └── Fix_CopyPawnKindBasics.README.md
│   └── CopyPawnKindTechHediffs/
│       ├── Fix_CopyPawnKindTechHediffs.cs
│       ├── Fix_CopyPawnKindTechHediffs.csproj
│       └── Fix_CopyPawnKindTechHediffs.README.md
└── README.md
```

## Requirements / Требования
- RimWorld 1.5 / 1.6
- Odyssey DLC
- Hardcore SK modpack
- Harmony
