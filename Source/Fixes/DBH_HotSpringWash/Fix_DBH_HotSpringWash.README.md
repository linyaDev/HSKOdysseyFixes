# Fix: DBH HotSpring — pawns don't wash in hot springs

## Bug / Баг

**EN:** Pawns refuse to wash in Odyssey DLC hot springs even though they are clean water. They will wash in regular shallow water but not in hot springs.

**RU:** Пешки отказываются мыться в горячих источниках DLC Odyssey, хотя это чистая вода. Они моются в обычном мелководье, но не в горячих источниках.

## Root Cause / Причина

**EN:** Dubs Bad Hygiene determines washable water by adding tags to TerrainDefs at startup in `DefExtensions.ResolveSurfaceWater()`. The method checks specific vanilla terrains and adds the `dbh_water` tag:

```csharp
// DefExtensions.cs line 222
if (item == TerrainDefOf.WaterShallow ||
    item == TerrainDefOf.WaterMovingShallow ||
    item == TerrainDefOf.WaterDeep ||
    item == TerrainDefOf.WaterMovingChestDeep)
{
    item.tags.Add("dbh_water");
}
```

Odyssey's `HotSpring` TerrainDef is not in this list, so it never gets the `dbh_water` tag. When `MapComponent_Hygiene` builds its water grid, HotSpring cells are not marked as clean water:

```csharp
// MapComponent_Hygiene.cs line 255
if (terrain.HasTag(DefExtensions.dbh_water))
{
    cleanWater.Set(c, true);  // HotSpring never reaches here
}
```

When a pawn needs to wash and no fixture (shower/bath) is available, `ClosestSanitation.FindBestHygieneSource()` falls back to `SanitationUtil.TryFindWaterCell()`, which checks `IsSurfaceWater()` — also tag-based. HotSpring cells fail this check.

**RU:** DBH определяет пригодность воды для мытья через теги TerrainDef, добавляемые при запуске в `DefExtensions.ResolveSurfaceWater()`. Метод проверяет конкретные ванильные террены и добавляет тег `dbh_water`. Террен `HotSpring` из Odyssey не входит в этот список, поэтому горячие источники не получают тег и не распознаются как чистая вода.

## Call Chain / Цепочка вызовов

```
Pawn needs to wash (Need_Hygiene low)
  → JobGiver_HaveWash.TryGiveJob()
    → ClosestSanitation.FindBestHygieneSource()
      → No fixtures found (no shower/bath)
      → SanitationUtil.TryFindWaterCell()
        → CellValidator: c.IsSurfaceWater()
          → Checks cleanWater area (built from dbh_water tag)
          → HotSpring has no dbh_water tag → NOT water
        → No valid cell found
      → Returns Invalid
    → Pawn doesn't wash
```

## Fix / Исправление

Harmony postfix on `DefExtensions.ResolveSurfaceWater()` adds `dbh_water` tag to the `HotSpring` TerrainDef after DBH finishes its own tagging.

## Verified / Проверено

Confirmed in both:
- HSK version of BadHygiene.dll (from game Mods folder)
- Latest GitHub version (github.com/Dubwise56/Dubs-Bad-Hygiene, 1.6/Assemblies)

Both have identical `ResolveSurfaceWater()` without HotSpring support.

## Proposed fix for BadHygiene.dll / Предложенный фикс для BadHygiene.dll

In `DefExtensions.ResolveSurfaceWater()`, add after the WaterShallow/WaterDeep check:

```csharp
var hotSpring = DefDatabase<TerrainDef>.GetNamedSilentFail("HotSpring");
if (hotSpring != null)
{
    if (hotSpring.tags == null)
        hotSpring.tags = new List<string>();
    hotSpring.tags.Add(dbh_water);
}
```
