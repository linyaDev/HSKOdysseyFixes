using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace HSKOdysseyFixes
{
    /// <summary>
    /// Fix: power cables render on top of power switch.
    ///
    /// Conduits use drawerType=MapMeshOnly with linked graphics baked into
    /// the map mesh. The switch also bakes into the same mesh, so altitudeLayer
    /// alone doesn't control draw order within the mesh.
    ///
    /// Fix: set drawerType=RealtimeOnly so the switch renders as a separate
    /// sprite on top of the map mesh, and set altitudeLayer=BuildingOnTop
    /// to ensure it draws above conduits.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Fix_PowerSwitchDrawOrder
    {
        static Fix_PowerSwitchDrawOrder()
        {
            var def = DefDatabase<ThingDef>.GetNamedSilentFail("PowerSwitch");
            if (def != null)
            {
                Log.Warning($"[PowerSwitchFix] BEFORE: drawerType={def.drawerType}, altitudeLayer={def.altitudeLayer}");
                def.drawerType = DrawerType.RealtimeOnly;
                def.altitudeLayer = AltitudeLayer.BuildingOnTop;
                Log.Warning($"[PowerSwitchFix] AFTER: drawerType={def.drawerType}, altitudeLayer={def.altitudeLayer}");
            }
            else
            {
                Log.Warning("[PowerSwitchFix] PowerSwitch ThingDef NOT FOUND!");
            }

            foreach (var td in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (td.defName.Contains("Conduit") || td.defName.Contains("Cable"))
                {
                    Log.Warning($"[PowerSwitchFix] {td.defName}: drawerType={td.drawerType}, altitudeLayer={td.altitudeLayer}, linkType={td.graphicData?.linkType}");
                }
            }
        }
    }
}
