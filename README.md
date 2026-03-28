# HSK Odyssey Fixes

Fixes for Odyssey DLC compatibility issues in Hardcore SK modpack.
Each fix is a separate DLL in `Assemblies/` with its own README in `Source/Fixes/`.

## Fixes / Фиксы

| Fix | DLL | Description |
|---|---|---|
| [CopyPawnKindBasics](Source/Fixes/Fix_CopyPawnKindBasics.README.md) | `Fix_CopyPawnKindBasics.dll` | Gravship scenario: all pawns spawn wearing vacsuits |

## Structure / Структура

```
HSKOdysseyFixes/
├── About/About.xml
├── Assemblies/
│   └── Fix_CopyPawnKindBasics.dll    ← each fix is a separate DLL
├── Source/Fixes/
│   ├── Fix_CopyPawnKindBasics.cs     ← fix code
│   ├── Fix_CopyPawnKindBasics.csproj ← project file
│   └── Fix_CopyPawnKindBasics.README.md  ← detailed description
└── README.md
```

## Requirements / Требования
- RimWorld 1.5 / 1.6
- Odyssey DLC
- Hardcore SK modpack
- Harmony
