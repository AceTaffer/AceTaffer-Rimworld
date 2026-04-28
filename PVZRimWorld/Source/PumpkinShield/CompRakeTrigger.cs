using RimWorld;
using Verse;
using System.Collections.Generic;

namespace PumpkinShield
{
    /// <summary>
    /// 钉耙陷阱的触发组件。
    /// 当敌对角色站在陷阱所在格时触发，对目标造成近战伤害，然后销毁陷阱。
    /// </summary>
    public class CompProperties_RakeTrigger : CompProperties
    {
        /// <summary>陷阱造成的近战伤害值</summary>
        public int damageAmount = 30;
        /// <summary>护甲穿透百分比（0~1）</summary>
        public float armorPenetration = 0.5f;
        /// <summary>伤害类型（默认钝击）</summary>
        public DamageDef damageDef;

        public CompProperties_RakeTrigger()
        {
            this.compClass = typeof(CompRakeTrigger);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (damageDef == null)
                damageDef = DamageDefOf.Blunt; // 默认钝击伤害
        }
    }

    public class CompRakeTrigger : ThingComp
    {
        public CompProperties_RakeTrigger Props => (CompProperties_RakeTrigger)props;

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Spawned) return;

            // 每帧检查陷阱所在格是否有敌对角色
            Pawn enemy = GetHostilePawnOnCell();
            if (enemy != null)
                TriggerRake(enemy);
        }

        /// <summary>获取陷阱所在格的敌对角色（非友军、非中立）</summary>
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

        /// <summary>触发钉耙，对敌人造成伤害并销毁陷阱</summary>
        private void TriggerRake(Pawn triggerer)
        {
            // 对触发者造成伤害
            DamageInfo dinfo = new DamageInfo(
                Props.damageDef,
                Props.damageAmount,
                Props.armorPenetration,
                -1f,
                parent
            );
            triggerer.TakeDamage(dinfo);

            // 销毁陷阱（一次性）
            parent.Destroy(DestroyMode.Vanish);
        }
    }
}