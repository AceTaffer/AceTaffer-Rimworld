using HarmonyLib;
using RimWorld;
using Verse;

namespace PumpkinShield
{
    /// <summary>
    /// Harmony 补丁：在近战攻击命中目标后，检查武器是否为小丑盒，若是则触发爆炸并销毁武器。
    /// 补丁目标：Verb_MeleeAttackDamage.ApplyMeleeDamageToTarget
    /// </summary>
    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "ApplyMeleeDamageToTarget")]
    public static class MeleeAttack_Patch
    {
        public static void Postfix(Verb_MeleeAttackDamage __instance, LocalTargetInfo target, DamageWorker.DamageResult __result)
        {
            // 如果目标无效，跳过
            if (!target.IsValid)
                return;

            // 获取攻击者
            Pawn caster = __instance.CasterPawn;
            if (caster == null)
                return;

            // 获取主武器
            ThingWithComps weapon = caster.equipment?.Primary;
            if (weapon == null)
                return;

            // 检查武器是否为小丑盒（根据 defName）
            if (weapon.def.defName != "MeleeWeapon_JokerBox")
                return;

            // 输出日志，确认补丁触发
            Log.Message("[PVZRimWorld] JokerBox: Melee attack detected, triggering explosion.");

            // 获取爆炸位置
            IntVec3 explosionPos = target.HasThing ? target.Thing.PositionHeld : target.Cell;
            Map map = caster.MapHeld;
            if (map == null)
                return;

            // 爆炸参数（可以从 XML 的 CompAttackExplode 读取，但为了简单，直接硬编码）
            float explosionRadius = 1.5f;
            int explosionDamage = 20;
            DamageDef damageDef = DamageDefOf.Bomb;

            // 触发爆炸
            GenExplosion.DoExplosion(
                explosionPos,
                map,
                explosionRadius,
                damageDef,
                caster,
                explosionDamage,
                -1f
            );
            Log.Message("[PVZRimWorld] JokerBox: Explosion triggered.");

            // 销毁武器
            if (!weapon.Destroyed)
            {
                Log.Message("[PVZRimWorld] JokerBox: Destroying weapon.");
                weapon.Destroy(DestroyMode.Vanish);
            }
        }
    }
}