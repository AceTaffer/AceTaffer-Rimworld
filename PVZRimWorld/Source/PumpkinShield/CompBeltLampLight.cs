using RimWorld;
using Verse;

namespace PumpkinShield
{
    public class CompProperties_BeltLampLight : CompProperties
    {
        public ThingDef lightSourceDef;
        public CompProperties_BeltLampLight()
        {
            this.compClass = typeof(CompBeltLampLight);
        }
    }

    public class CompBeltLampLight : ThingComp
    {
        private Thing lightSource;

        public CompProperties_BeltLampLight Props => (CompProperties_BeltLampLight)props;

        private Pawn Wearer => (parent as Apparel)?.Wearer;

        public override void Notify_Equipped(Pawn wearer)
        {
            base.Notify_Equipped(wearer);
            if (lightSource == null && wearer.Spawned && wearer.Map != null)
            {
                lightSource = GenSpawn.Spawn(Props.lightSourceDef, wearer.Position, wearer.Map);
            }
        }

        public override void Notify_Unequipped(Pawn wearer)
        {
            base.Notify_Unequipped(wearer);
            if (lightSource != null && lightSource.Spawned)
            {
                lightSource.Destroy();
                lightSource = null;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            Pawn wearer = Wearer;
            if (lightSource != null && lightSource.Spawned && wearer != null && wearer.Spawned)
            {
                // 如果光源位置与穿戴者不同，则移动光源
                if (lightSource.Position != wearer.Position)
                {
                    // 先移除，再在新位置生成同一个光源实例
                    lightSource.DeSpawn();
                    GenSpawn.Spawn(lightSource, wearer.Position, wearer.Map);
                }
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (lightSource != null && lightSource.Spawned)
            {
                lightSource.Destroy();
                lightSource = null;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref lightSource, "lightSource");
        }
    }
}