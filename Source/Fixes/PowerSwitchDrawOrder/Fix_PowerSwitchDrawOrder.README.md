# Fix: PowerSwitch draw order — cables render on top

## Bug / Баг

**EN:** Power cables/conduits visually render on top of the power switch when placed on adjacent tiles. The cable's linked graphic draws a connection toward the switch, overlapping its texture.

**RU:** Силовые кабели/кондуиты визуально рисуются поверх выключателя при размещении на соседних клетках. Linked-графика кабеля рисует соединение в сторону свитча, перекрывая его текстуру.

## Root Cause / Причина

### How RimWorld renders buildings / Как RimWorld рендерит здания

RimWorld uses two rendering methods:
1. **MapMeshOnly** — object is baked into the map mesh (one large texture). Fast, but no per-object draw order control. This is how conduits render.
2. **RealtimeOnly** — object is drawn every frame as a separate sprite. Altitude layer controls draw order. This is how pawns and items render.

RimWorld использует два метода рендеринга:
1. **MapMeshOnly** — объект запекается в меш карты (одна большая текстура). Быстро, но нет контроля порядка рисования отдельных объектов. Так рисуются кондуиты.
2. **RealtimeOnly** — объект рисуется каждый кадр как отдельный спрайт. Altitude layer контролирует порядок. Так рисуются пешки и предметы.

### The problem / Проблема

Both PowerConduit and PowerSwitch use `drawerType: MapMeshOnly`. They are both baked into the **same map mesh**. Within this mesh, draw order is determined by tile bake order, not by `altitudeLayer`. The conduit's linked graphic extends into the switch's tile and may be baked **after** the switch texture — appearing on top.

PowerConduit и PowerSwitch оба используют `drawerType: MapMeshOnly`. Они запекаются в **один и тот же меш карты**. Внутри этого меша порядок рисования определяется порядком запекания тайлов, а не `altitudeLayer`. Linked-графика кондуита заходит на клетку свитча и может запечься **после** текстуры свитча — оказываясь поверх.

### Why altitudeLayer alone doesn't help / Почему altitudeLayer не помогает

Changing `altitudeLayer` to `BuildingOnTop` has no effect when both objects use `MapMeshOnly` — they share the same mesh and altitude only matters for separately drawn sprites.

Изменение `altitudeLayer` на `BuildingOnTop` не помогает когда оба объекта используют `MapMeshOnly` — они в одном меше, и altitude влияет только на отдельно рисуемые спрайты.

### HSK specifics / Особенности HSK

HSK's `Building_PowerSwitchMod.DestroyOtherPowerTransmitter()` destroys conduits placed directly under the switch, forcing conduits to be adjacent — making the linked graphic overlap more visible than in vanilla.

`Building_PowerSwitchMod.DestroyOtherPowerTransmitter()` в HSK уничтожает кондуиты под свитчем, заставляя ставить их рядом — что делает перекрытие linked-графики более заметным, чем в ванилле.

## Fix / Исправление

C# fix via `[StaticConstructorOnStartup]` — changes two PowerSwitch ThingDef fields at game startup:

1. `drawerType = RealtimeOnly` — switch is no longer baked into the map mesh, it renders as a separate sprite every frame
2. `altitudeLayer = BuildingOnTop` — switch renders above conduits, buildings, and other baked objects

This combination ensures the switch always draws on top of conduit linked graphics.

C# фикс через `[StaticConstructorOnStartup]` — меняет два поля ThingDef PowerSwitch при запуске игры:

1. `drawerType = RealtimeOnly` — свитч больше не запекается в меш карты, рисуется как отдельный спрайт каждый кадр
2. `altitudeLayer = BuildingOnTop` — свитч рендерится выше кондуитов, зданий и других запечённых объектов

Эта комбинация гарантирует что свитч всегда рисуется поверх linked-графики кондуитов.

### Why XML-only fix doesn't work / Почему XML-фикс не работает

XML patch can change `altitudeLayer` but cannot change `drawerType` — it's set by `BuildingBase` parent def and overridden by the mesh system. Only C# can change it at runtime after all defs are loaded.

XML патч может изменить `altitudeLayer`, но не может изменить `drawerType` — он задаётся родительским дефом `BuildingBase` и переопределяется системой мешей. Только C# может изменить его в рантайме после загрузки всех дефов.
