using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace PumpkinShield
{
    /// <summary>
    /// 植物射击组件属性类。
    /// 定义射程、射击间隔、子弹类型等。
    /// </summary>
    public class CompProperties_PlantShooter : CompProperties
    {
        /// <summary>攻击半径（格）</summary>
        public float range = 25f;
        /// <summary>每次连发数量</summary>
        public int burstShotCount = 1;
        /// <summary>每发之间的间隔（ticks）</summary>
        public int ticksBetweenShots = 300;
        /// <summary>使用的子弹 defName</summary>
        public ThingDef projectileDef;
        /// <summary>各距离精度（覆盖原版精度计算）</summary>
        public float accuracyTouch = 1f;
        public float accuracyShort = 1f;
        public float accuracyMedium = 1f;
        public float accuracyLong = 1f;
        /// <summary>
        /// 是否检查视线（墙体阻挡）。true 表示只攻击可见敌人；false 表示无视墙体。
        /// 默认为 false，保持原有行为。
        /// </summary>
        public bool checkLineOfSight = false;

        public CompProperties_PlantShooter()
        {
            this.compClass = typeof(CompPlantShooter);
        }
    }

    /// <summary>
    /// 植物射击组件。
    /// 每帧检查冷却和敌人，当冷却为0时寻找敌对目标并发射子弹。
    /// 仅当植物完全成熟（Growth >= 1.0）时才攻击。
    /// </summary>
    public class CompPlantShooter : ThingComp
    {
        public CompProperties_PlantShooter Props => (CompProperties_PlantShooter)props;

        /// <summary>当前冷却剩余 ticks</summary>
        private int cooldownTicks;

        public override void CompTick()
        {
            base.CompTick();

            // 植物未生成则返回
            if (parent == null || !parent.Spawned)
                return;

            // 获取植物组件，检查是否成熟
            Plant plant = parent as Plant;
            if (plant == null)
                return;

            // 原版 Plant 没有 IsHarvestable 方法，使用 Growth 判断是否完全成熟
            if (plant.Growth < 1.0f)
                return; // 未成熟，不攻击

            // 减少冷却
            if (cooldownTicks > 0)
            {
                cooldownTicks--;
                return;
            }

            // 寻找目标（根据 checkLineOfSight 决定是否检查视线）
            if (TryFindTarget(out LocalTargetInfo target))
            {
                // 执行连发
                for (int i = 0; i < Props.burstShotCount; i++)
                {
                    ShootAt(target);
                }

                // 重置冷却（总冷却 = 每发间隔 × 连发次数）
                cooldownTicks = Props.ticksBetweenShots * Props.burstShotCount;
            }
        }

        /// <summary>
        /// 在射程内寻找敌对目标。
        /// 如果 checkLineOfSight 为 true，则只返回有视线的目标；否则无视墙体。
        /// 策略：选择最近的一个敌对角色（满足视线条件时）。
        /// </summary>
        private bool TryFindTarget(out LocalTargetInfo target)
        {
            target = LocalTargetInfo.Invalid;

            IntVec3 position = parent.Position;
            Map map = parent.Map;

            // 获取所有已生成且存活的敌对角色
            IEnumerable<Pawn> enemies = map.mapPawns.AllPawnsSpawned
                .Where(p => p.HostileTo(Faction.OfPlayer) && !p.Downed && p.Spawned);

            // 筛选在射程内的敌人
            var inRange = enemies.Where(p => p.Position.DistanceTo(position) <= Props.range)
                                  .OrderBy(p => p.Position.DistanceToSquared(position));

            foreach (Pawn candidate in inRange)
            {
                // 如果需要检查视线，且没有视线，则跳过
                if (Props.checkLineOfSight && !GenSight.LineOfSight(position, candidate.Position, map, false))
                    continue;

                target = candidate;
                return true;
            }

            return false; // 没有符合条件的敌人
        }

        /// <summary>
        /// 向目标发射子弹。
        /// </summary>
        private void ShootAt(LocalTargetInfo target)
        {
            // 如果目标无效（例如敌人刚死亡），则返回
            if (!target.IsValid)
                return;

            // 生成投射物
            Projectile projectile = (Projectile)GenSpawn.Spawn(Props.projectileDef, parent.Position, parent.Map);

            // 使用 All 标志，确保子弹命中目标实体
            projectile.Launch(
                parent,
                target,
                target,
                ProjectileHitFlags.All
            );
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref cooldownTicks, "cooldownTicks", 0);
        }
    }
}