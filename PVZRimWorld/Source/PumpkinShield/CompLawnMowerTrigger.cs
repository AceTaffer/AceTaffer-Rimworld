using RimWorld;
using Verse;

namespace PumpkinShield
{
    public class CompProperties_LawnMowerTrigger : CompProperties
    {
        public ThingDef projectileDef;
        public bool triggerOnSpawn = false;

        public CompProperties_LawnMowerTrigger()
        {
            this.compClass = typeof(CompLawnMowerTrigger);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (projectileDef == null)
                Log.Error("[PVZRimWorld] CompLawnMowerTrigger: projectileDef is not set!");
        }
    }

    public class CompLawnMowerTrigger : ThingComp
    {
        public CompProperties_LawnMowerTrigger Props => (CompProperties_LawnMowerTrigger)props;

        private int checkInterval = 15;
        private int ticksSinceLastCheck = 0;
        private bool triggerQueued = false;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (Props.triggerOnSpawn && !respawningAfterLoad)
                triggerQueued = true;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (triggerQueued)
            {
                triggerQueued = false;
                TriggerLawnMower();
                return;
            }

            ticksSinceLastCheck++;
            if (ticksSinceLastCheck < checkInterval) return;
            ticksSinceLastCheck = 0;

            if (!parent.Spawned) return;

            Pawn enemy = GetHostilePawnOnCell();
            if (enemy != null)
                TriggerLawnMower();
        }

        private Pawn GetHostilePawnOnCell()
        {
            var things = parent.Map.thingGrid.ThingsListAt(parent.Position);
            foreach (Thing thing in things)
            {
                Pawn pawn = thing as Pawn;
                if (pawn != null && !pawn.Dead && pawn.HostileTo(Faction.OfPlayer))
                    return pawn;
            }
            return null;
        }

        private void TriggerLawnMower()
        {
            Rot4 dir = parent.Rotation;
            IntVec3 spawnPos = parent.Position;

            Thing spawned = GenSpawn.Spawn(Props.projectileDef, spawnPos, parent.Map);

            if (spawned is Projectile_NutBowl nut)
                nut.SetDirection(dir);
            else if (spawned is Projectile projectile)
            {
                IntVec3 targetCell = spawnPos + dir.FacingCell * 9999;
                projectile.Launch(parent, targetCell, targetCell, ProjectileHitFlags.None);
            }
            else
            {
                Log.Error($"[PVZRimWorld] Invalid projectile type spawned: {spawned.GetType()}");
                spawned.Destroy();
            }

            parent.Destroy(DestroyMode.Vanish);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref triggerQueued, "triggerQueued", false);
        }
    }
}