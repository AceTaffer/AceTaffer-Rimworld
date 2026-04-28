using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;  // 添加 ToList 扩展方法

namespace PumpkinShield
{
    public class Projectile_LawnMower : Projectile
    {
        // 伤害参数（可从XML读取，这里保持硬编码）
        private int damageAmount = 60;
        private float armorPenetration = 1f; // 100%
        private DamageDef damageDef = DamageDefOf.Blunt;

        // 记录已经伤害过的角色，避免重复伤害同一目标
        private HashSet<Pawn> alreadyDamagedPawns = new HashSet<Pawn>();

        protected override void Tick()
        {
            base.Tick(); // 基类处理移动逻辑

            if (!this.Spawned) return;

            // 获取当前格子上的所有事物
            List<Thing> things = this.Map.thingGrid.ThingsListAt(this.Position);
            if (things.Count == 0) return;

            // ★ 关键修正：复制一份列表，防止在遍历时原集合被修改 ★
            foreach (Thing thing in things.ToList())
            {
                Pawn pawn = thing as Pawn;
                if (pawn != null && !pawn.Dead && pawn.HostileTo(Faction.OfPlayer))
                {
                    if (!alreadyDamagedPawns.Contains(pawn))
                    {
                        DamageInfo dinfo = new DamageInfo(damageDef, damageAmount, armorPenetration, -1f, this.launcher);
                        pawn.TakeDamage(dinfo);
                        alreadyDamagedPawns.Add(pawn);
                    }
                }
            }
        }

        // 当投射物停止（如撞到障碍、到达目标）时自动销毁
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            this.Destroy(DestroyMode.Vanish);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref alreadyDamagedPawns, "alreadyDamagedPawns", LookMode.Reference);
        }
    }
}