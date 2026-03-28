# HSK Odyssey Fixes

Fixes for Odyssey DLC compatibility issues in Hardcore SK modpack.
Each fix is a separate DLL in `Assemblies/` with its own README in `Source/Fixes/`.

## Fixes / Фиксы

| Fix | DLL | Description |
|---|---|---|
| [CopyPawnKindBasics](Source/Fixes/Fix_CopyPawnKindBasics.README.md) | `Fix_CopyPawnKindBasics.dll` | Gravship scenario: all pawns spawn wearing vacsuits |
| [CopyPawnKindTechHediffs](Source/Fixes/Fix_CopyPawnKindTechHediffs.README.md) | `Fix_CopyPawnKindTechHediffs.dll` | Prevents random mechlink on scenario pawns |

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
