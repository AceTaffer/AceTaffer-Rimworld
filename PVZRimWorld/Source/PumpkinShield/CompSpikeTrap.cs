using RimWorld;
using Verse;
using System.Collections.Generic;

namespace PumpkinShield
{
    /// <summary>
    /// 地刺组件的属性类。
    /// 定义伤害值、伤害类型、伤害间隔、最小激活生长度。
    /// </summary>
    public class CompProperties_SpikeTrap : CompProperties
    {
        /// <summary>每次伤害的数值</summary>
        public int damageAmount = 15;
        /// <summary>伤害类型（如 Stab、Cut 等）</summary>
        public DamageDef damageDef;
        /// <summary>伤害间隔（ticks）</summary>
        public int damageIntervalTicks = 120; // 默认 2 秒
        /// <summary>触发伤害所需的最小生长进度（0.0~1.0）</summary>
        public float minGrowthToActivate = 1.0f;

        public CompProperties_SpikeTrap()
        {
            this.compClass = typeof(CompSpikeTrap);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (damageDef == null)
                damageDef = DamageDefOf.Stab; // 默认刺伤
        }
    }

    /// <summary>
    /// 地刺的核心逻辑组件。
    /// 每帧检查是否有敌对角色站在自己所在的格子上，如果成熟且冷却为0，则对其造成伤害并重置冷却。
    /// </summary>
    public class CompSpikeTrap : ThingComp
    {
        public CompProperties_SpikeTrap Props => (CompProperties_SpikeTrap)props;

        /// <summary>距离下次伤害剩余的 ticks</summary>
        private int remainingCooldownTicks = 0;

        public override void CompTick()
        {
            base.CompTick();

            // 如果植物未生成，则返回
            if (parent == null || !parent.Spawned)
                return;

            // 获取植物组件，检查是否成熟
            Plant plant = parent as Plant;
            if (plant == null)
                return;

            // 如果生长进度未达到要求，则不激活
            if (plant.Growth < Props.minGrowthToActivate)
                return;

            // 冷却递减
            if (remainingCooldownTicks > 0)
            {
                remainingCooldownTicks--;
                return;
            }

            // 寻找站在本格上的敌对角色
            Pawn target = GetHostilePawnOnCell();
            if (target != null)
            {
                // 造成伤害
                DamageInfo dinfo = new DamageInfo(
                    Props.damageDef,
                    Props.damageAmount,
                    0f, // 护甲穿透（0 表示无穿透）
                    -1f, // 角度（默认）
                    parent // 伤害来源
                );
                target.TakeDamage(dinfo);

                // 重置冷却
                remainingCooldownTicks = Props.damageIntervalTicks;
            }
        }

        /// <summary>
        /// 获取站在本格上的敌对角色（非友军、非中立、未死亡）。
        /// </summary>
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

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref remainingCooldownTicks, "remainingCooldownTicks", 0);
        }
    }
}