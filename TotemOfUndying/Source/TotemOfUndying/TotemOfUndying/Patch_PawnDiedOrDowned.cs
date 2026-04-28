using HarmonyLib;
using RimWorld;
using Verse;
using System.Linq;

namespace TotemOfUndying
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Patch_PawnKill
    {
        public static bool Prefix(Pawn __instance, DamageInfo? dinfo)
        {
            var totem = __instance.apparel?.WornApparel.FirstOrDefault(a => a.def == ThingDef.Named("TotemOfUndying"));
            if (totem != null)
            {
                var comp = totem.TryGetComp<CompTotem>();
                if (comp != null && !comp.Triggered)
                {
                    comp.TriggerEffect(__instance);
                    return false;
                }
            }
            return true;
        }
    }
}