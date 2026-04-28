using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace PumpkinShield
{
    /// <summary>
    /// 坚果保龄球投射物。
    /// 移动速度：每 3 tick 移动一格 → 20 格/秒（流畅快速），反弹最多 5 次后销毁。
    /// 旋转由 Harmony 补丁 + CompNutBowlRotator 实现。
    /// </summary>
    public class Projectile_NutBowl : ThingWithComps
    {
        // 伤害参数
        private int damageAmount = 100;
        private float armorPenetration = 1f;
        private DamageDef damageDef = DamageDefOf.Blunt;

        // 状态数据
        private HashSet<Pawn> alreadyDamagedPawns = new HashSet<Pawn>();
        private Rot4 direction;
        private int moveCooldown = 0;
        private const int MoveCooldownMax = 3;          // 每 3 tick 移动一格 → 20 格/秒
        private int bounceCount = 0;
        private const int MaxBounces = 5;

        public void SetDirection(Rot4 dir) => this.direction = dir;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            moveCooldown = Rand.Range(0, MoveCooldownMax);
        }

        protected override void Tick()
        {
            base.Tick();
            if (!Spawned) return;

            // 更新旋转（由组件管理）
            var rotComp = this.GetComp<CompNutBowlRotator>();
            if (rotComp != null)
            {
                rotComp.UpdateRotation();
            }

            // 移动冷却
            if (moveCooldown > 0)
            {
                moveCooldown--;
                return;
            }
            moveCooldown = MoveCooldownMax;

            TryMove();
        }

        private void TryMove()
        {
            IntVec3 nextPos = Position + direction.FacingCell;

            if (!nextPos.InBounds(Map))
            {
                Destroy();
                return;
            }

            if (!CanEnter(nextPos))
            {
                bounceCount++;
                if (bounceCount >= MaxBounces)
                {
                    Destroy();
                    return;
                }

                direction = direction.Opposite;
                nextPos = Position + direction.FacingCell;

                if (!nextPos.InBounds(Map) || !CanEnter(nextPos))
                {
                    Destroy();
                    return;
                }
            }

            Position = nextPos;
            CheckDamage();
        }

        private bool CanEnter(IntVec3 cell)
        {
            if (!cell.InBounds(Map)) return false;
            Building edifice = cell.GetEdifice(Map);
            return edifice == null || edifice.def.passability != Traversability.Impassable;
        }

        private void CheckDamage()
        {
            // 使用 ToList() 创建副本，避免遍历时集合被修改
            List<Thing> things = Map.thingGrid.ThingsListAt(Position).ToList();
            foreach (Thing thing in things)
            {
                Pawn pawn = thing as Pawn;
                if (pawn != null && !pawn.Dead && pawn.HostileTo(Faction.OfPlayer))
                {
                    if (!alreadyDamagedPawns.Contains(pawn))
                    {
                        DamageInfo dinfo = new DamageInfo(damageDef, damageAmount, armorPenetration, -1f, this);
                        pawn.TakeDamage(dinfo);
                        alreadyDamagedPawns.Add(pawn);

                        if (Rand.Chance(0.5f))
                        {
                            RotationDirection rotDir = Rand.Bool ? RotationDirection.Clockwise : RotationDirection.Counterclockwise;
                            direction = direction.Rotated(rotDir);
                        }
                    }
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref damageAmount, "damageAmount", 100);
            Scribe_Values.Look(ref armorPenetration, "armorPenetration", 1f);
            Scribe_Defs.Look(ref damageDef, "damageDef");
            Scribe_Collections.Look(ref alreadyDamagedPawns, "alreadyDamagedPawns", LookMode.Reference);
            Scribe_Values.Look(ref direction, "direction");
            Scribe_Values.Look(ref moveCooldown, "moveCooldown", 0);
            Scribe_Values.Look(ref bounceCount, "bounceCount", 0);
        }
    }
}