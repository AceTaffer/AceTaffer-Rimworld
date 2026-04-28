using RimWorld;
using Verse;

namespace PumpkinShield
{
    public class Verb_MeleeAttack_Explode : Verb_MeleeAttackDamage
    {
        // 静态构造函数，在类加载时输出日志（可选）
        static Verb_MeleeAttack_Explode()
        {
            Log.Message("[PVZRimWorld] Verb_MeleeAttack_Explode static constructor called.");
        }

        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            // 记录进入方法
            Log.Message("[PVZRimWorld] JokerBox: ApplyMeleeDamageToTarget CALLED.");

            // 调用基类方法，造成近战伤害
            DamageWorker.DamageResult result = base.ApplyMeleeDamageToTarget(target);
            Log.Message("[PVZRimWorld] JokerBox: Base melee damage applied.");

            // 检查目标有效性
            if (!target.IsValid)
            {
                Log.Warning("[PVZRimWorld] JokerBox: Target is invalid, explosion skipped.");
                return result;
            }

            // 获取爆炸位置
            IntVec3 explosionPos = target.HasThing ? target.Thing.PositionHeld : target.Cell;
            Map map = CasterPawn?.MapHeld;
            if (map == null)
            {
                Log.Error("[PVZRimWorld] JokerBox: Map is null, explosion skipped.");
                return result;
            }

            Log.Message($"[PVZRimWorld] JokerBox: Explosion at {explosionPos}, map exists. Preparing explosion...");

            // 硬编码爆炸参数
            float explosionRadius = 1.5f;
            int explosionDamage = 20;
            DamageDef damageDef = DamageDefOf.Bomb;

            // 触发爆炸
            GenExplosion.DoExplosion(
                explosionPos,
                map,
                explosionRadius,
                damageDef,
                CasterPawn,
                explosionDamage,
                -1f
            );
            Log.Message("[PVZRimWorld] JokerBox: Explosion triggered.");

            // 销毁武器（一次性）
            ThingWithComps weapon = CasterPawn?.equipment?.Primary;
            if (weapon != null && !weapon.Destroyed)
            {
                Log.Message($"[PVZRimWorld] JokerBox: Destroying weapon {weapon.def.defName}.");
                weapon.Destroy(DestroyMode.Vanish);
            }
            else
            {
                Log.Warning("[PVZRimWorld] JokerBox: Weapon not found or already destroyed.");
            }

            return result;
        }
    }
}