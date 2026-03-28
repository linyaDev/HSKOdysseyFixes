# Fix: PowerSwitch draw order — cables render on top

## Bug / Баг

**EN:** Power cables/conduits visually render on top of the power switch when placed on adjacent tiles. The cable's linked graphic draws a connection toward the switch, overlapping its texture.

**RU:** Силовые кабели/кондуиты визуально рисуются поверх выключателя при размещении на соседних клетках. Linked-графика кабеля рисует соединение в сторону свитча, перекрывая его текстуру.

## Root Cause / Причина

**EN:** Power conduits use `Graphic_Single` with `linkType: Transmitter`. When a conduit is adjacent to a switch (which has `CompPowerTransmitter`), the conduit draws a linked connection toward the switch's tile. The conduit renders at `altitudeLayer: Conduits`, and the switch at `altitudeLayer: Building`. While Building is above Conduits in the render order, the conduit's linked texture extends into the switch's tile and visually overlaps it due to texture size.

Additionally, HSK's `Building_PowerSwitchMod.DestroyOtherPowerTransmitter()` destroys any conduit placed directly under the switch, so conduits can only be adjacent — making the linked graphic overlap more visible.

**RU:** Кондуиты используют `Graphic_Single` с `linkType: Transmitter`. Когда кондуит рядом с свитчем (у которого есть `CompPowerTransmitter`), кондуит рисует linked-соединение на клетку свитча. Кондуит рендерится на `altitudeLayer: Conduits`, свитч на `altitudeLayer: Building`. Хотя Building выше Conduits, текстура кондуита залезает на клетку свитча.

Кроме того, `Building_PowerSwitchMod.DestroyOtherPowerTransmitter()` в HSK уничтожает кондуит под свитчем, поэтому кондуиты могут быть только рядом — что делает перекрытие linked-графики более заметным.

## Fix / Исправление

**XML patch** — raise `altitudeLayer` from `Building` to `BuildingOnTop`. This is a vanilla-safe layer already used by furniture that sits on top of other buildings.

**XML патч** — поднимает `altitudeLayer` с `Building` на `BuildingOnTop`. Это безопасный слой, уже используемый ванильной мебелью поверх построек.

No DLL required — pure XML fix in `Patches/Patch_PowerSwitch_DrawOrder.xml`.
