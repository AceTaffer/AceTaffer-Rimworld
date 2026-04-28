using RimWorld;
using Verse;
using System.Collections.Generic;

namespace PumpkinShield
{
    public class CompProperties_MineTrigger : CompProperties
    {
        public float explosionRadius = 2.9f;
        public int explosionDamage = 50;
        public DamageDef explosionDamageDef;   // 不再赋默认值
        public float minGrowthToTrigger = 1.0f;
        public bool destroyOnTrigger = true;

        public CompProperties_MineTrigger()
        {
            this.compClass = typeof(CompMineTrigger);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (explosionDamageDef == null)
                explosionDamageDef = DamageDefOf.Bomb;
        }
    }

    public class CompMineTrigger : ThingComp
    {
        public CompProperties_MineTrigger Props => (CompProperties_MineTrigger)props;

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Spawned) return;

            Plant plant = parent as Plant;
            if (plant == null) return;
            if (plant.Growth < Props.minGrowthToTrigger) return;

            Pawn enemy = GetHostilePawnOnCell();
            if (enemy != null)
                TriggerMine(enemy);
        }

        private Pawn GetHostilePawnOnCell()
        {
            List<Thing> things = parent.Map.thingGrid.ThingsListAt(parent.Position);
            foreach (Thing thing in things)
            {
                Pawn pawn = thing as Pawn;
                if (pawn == null || pawn.Dead) continue;
                if (pawn.HostileTo(Faction.OfPlayer))
                    return pawn;
            }
            return null;
        }

        private void TriggerMine(Pawn triggerer)
        {
            GenExplosion.DoExplosion(
                parent.PositionHeld,
                parent.MapHeld,
                Props.explosionRadius,
                Props.explosionDamageDef,
                null,
                Props.explosionDamage,
                -1f
            );

            if (Props.destroyOnTrigger)
                parent.Destroy(DestroyMode.Vanish);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}