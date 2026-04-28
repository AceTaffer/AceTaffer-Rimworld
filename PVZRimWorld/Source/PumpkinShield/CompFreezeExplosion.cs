using RimWorld;
using Verse;
using System.Collections.Generic;

namespace PumpkinShield
{
    // ========== 寒冰菇自定义植物类 ==========
    /// <summary>
    /// 寒冰菇的自定义植物类，用于标记是否因收获而销毁。
    /// 当殖民者收获时，设置 wasHarvested = true，爆炸组件据此跳过爆炸。
    /// </summary>
    public class Plant_IceShroom : Plant
    {
        private bool wasHarvested = false;

        public override void PlantCollected(Pawn by, PlantDestructionMode mode)
        {
            wasHarvested = true;          // 标记为收获
            base.PlantCollected(by, mode); // 调用基类方法，完成默认收获逻辑
        }

        public bool WasHarvested => wasHarvested;
    }

    // ========== 眩晕爆炸组件属性 ==========
    /// <summary>
    /// 寒冰菇死亡时触发眩晕爆炸的组件属性。
    /// </summary>
    public class CompProperties_FreezeExplosion : CompProperties
    {
        /// <summary>爆炸半径（格）</summary>
        public float explosionRadius = 5.9f;

        /// <summary>眩晕持续时间（秒）</summary>
        public float stunDurationSeconds = 10f;

        /// <summary>冻伤伤害最小值</summary>
        public int damageMin = 10;

        /// <summary>冻伤伤害最大值</summary>
        public int damageMax = 35;

        /// <summary>伤害类型（冻伤）。默认设为 null，在 ResolveReferences 中赋值以避免 DefOf 未初始化问题。</summary>
        public DamageDef damageDef;

        /// <summary>触发爆炸所需的最小生长进度</summary>
        public float minGrowthToTrigger = 0.8f;

        /// <summary>爆炸后是否销毁植物</summary>
        public bool destroyOnTrigger = true;

        public CompProperties_FreezeExplosion()
        {
            this.compClass = typeof(CompFreezeExplosion);
        }

        /// <summary>
        /// 在所有 Def 加载完成后解析引用。在此处设置默认伤害类型，确保此时 DamageDefOf 已初始化。
        /// </summary>
        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (damageDef == null)
                damageDef = DamageDefOf.Frostbite; // 安全使用已初始化的 DefOf
        }
    }

    // ========== 眩晕爆炸组件 ==========
    /// <summary>
    /// 寒冰菇死亡时，对周围所有生物施加眩晕和冻伤伤害。收获时不触发。
    /// </summary>
    public class CompFreezeExplosion : ThingComp
    {
        public CompProperties_FreezeExplosion Props => (CompProperties_FreezeExplosion)props;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            // 获取地图
            Map map = parent.MapHeld ?? previousMap;
            if (map == null) return;

            // 获取植物组件
            Plant plant = parent as Plant;
            if (plant == null) return;

            // 生长进度不足时不触发
            if (plant.Growth < Props.minGrowthToTrigger) return;

            // 🔥 关键判断：如果是寒冰菇且因收获而销毁，则不爆炸
            if (parent is Plant_IceShroom iceShroom && iceShroom.WasHarvested)
                return;

            // 执行眩晕爆炸效果
            TriggerFreezeExplosion(map);
        }

        /// <summary>
        /// 实际触发眩晕爆炸
        /// </summary>
        private void TriggerFreezeExplosion(Map map)
        {
            // 获取爆炸中心
            IntVec3 center = parent.PositionHeld;

            // 获取爆炸半径内的所有事物
            IEnumerable<Thing> things = GenRadial.RadialDistinctThingsAround(center, map, Props.explosionRadius, true);

            foreach (Thing thing in things)
            {
                // 只对生物（Pawn）生效
                Pawn pawn = thing as Pawn;
                if (pawn == null || pawn.Dead) continue;

                // 1. 施加眩晕效果
                int stunTicks = (int)(Props.stunDurationSeconds * 60f); // 1秒=60ticks
                pawn.stances.stunner.StunFor(stunTicks, parent);

                // 2. 施加随机冻伤伤害
                int damage = Rand.Range(Props.damageMin, Props.damageMax);
                DamageInfo dinfo = new DamageInfo(Props.damageDef, damage, 0, -1, parent);
                pawn.TakeDamage(dinfo);
            }

            // 如果需要销毁植物（通常已在销毁流程中，但以防万一）
            if (Props.destroyOnTrigger && !parent.Destroyed)
            {
                parent.Destroy(DestroyMode.Vanish);
            }
        }
    }
}