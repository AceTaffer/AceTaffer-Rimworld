using RimWorld;
using Verse;

namespace PumpkinShield
{
    // ========== 攻击爆炸组件属性 ==========
    public class CompProperties_AttackExplode : CompProperties
    {
        /// <summary>爆炸半径（格）</summary>
        public float explosionRadius = 1.5f;

        /// <summary>爆炸伤害值</summary>
        public int explosionDamage = 20;

        /// <summary>爆炸伤害类型</summary>
        public DamageDef explosionDamageDef;

        /// <summary>使用后是否销毁武器</summary>
        public bool destroyWeaponOnUse = true;

        public CompProperties_AttackExplode()
        {
            this.compClass = typeof(CompAttackExplode);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (explosionDamageDef == null)
                explosionDamageDef = DamageDefOf.Bomb;
        }
    }

    // ========== 攻击爆炸组件（仅存储数据，由自定义 Verb 触发爆炸）==========
    public class CompAttackExplode : ThingComp
    {
        public CompProperties_AttackExplode Props => (CompProperties_AttackExplode)props;

        /// <summary>
        /// 执行爆炸，由自定义 Verb 调用
        /// </summary>
        public void DoExplosion(LocalTargetInfo target, Pawn attacker)
        {
            // 确定爆炸位置
            IntVec3 explosionPos = target.HasThing ? target.Thing.PositionHeld : target.Cell;
            Map map = attacker.MapHeld;
            if (map == null) return;

            // 触发爆炸
            GenExplosion.DoExplosion(
                explosionPos,
                map,
                Props.explosionRadius,
                Props.explosionDamageDef,
                attacker,
                Props.explosionDamage,
                -1f
            );

            // 销毁武器
            if (Props.destroyWeaponOnUse && parent != null && !parent.Destroyed)
            {
                parent.Destroy(DestroyMode.Vanish);
            }
        }
    }
}