using HarmonyLib;
using RimWorld;
using Verse;

namespace PumpkinShield
{
    /// <summary>
    /// Harmony 补丁：为所有 Bullet.Impact 方法添加后处理逻辑。
    /// 根据子弹 defName 前缀处理：
    ///   - "Bullet_Ice"：施加眩晕和冻伤
    ///   - "Bullet_Fire"：施加烧伤（点燃效果）
    /// </summary>
    [HarmonyPatch(typeof(Bullet), "Impact")]
    public static class Bullet_Impact_Patch
    {
        public static void Postfix(Bullet __instance, Thing hitThing)
        {
            if (__instance == null || hitThing == null) return;

            // 处理寒冰子弹（包括寒冰射手）
            if (__instance.def.defName.StartsWith("Bullet_Ice"))
            {
                ApplyIceEffects(__instance, hitThing, __instance.Launcher);
            }
            // 处理烈焰子弹
            else if (__instance.def.defName.StartsWith("Bullet_Fire"))
            {
                ApplyFireEffects(__instance, hitThing, __instance.Launcher);
            }
        }

        /// <summary>
        /// 寒冰子弹效果：眩晕 + 冻伤伤害
        /// </summary>
        private static void ApplyIceEffects(Bullet bullet, Thing victim, Thing source)
        {
            if (victim == null || !victim.Spawned) return;
            Pawn pawn = victim as Pawn;
            if (pawn == null || pawn.Dead) return;

            // 根据子弹 defName 设置参数
            float stunSeconds = 0f;        // 默认无眩晕
            int extraDamage = 0;

            // 寒冰武器系列
            if (bullet.def.defName == "Bullet_IcePistol")
            {
                stunSeconds = 0.5f;
                extraDamage = 1;
            }
            else if (bullet.def.defName == "Bullet_IceRifle")
            {
                stunSeconds = 0.5f;
                extraDamage = 3;
            }
            else if (bullet.def.defName == "Bullet_IceSniper")
            {
                stunSeconds = 0.5f;
                extraDamage = 5;
            }
            // 寒冰射手子弹
            else if (bullet.def.defName == "Bullet_IcePea")
            {
                // 寒冰射手：无眩晕，额外2点冻伤
                stunSeconds = 0f;
                extraDamage = 2;
            }
            // 在现有寒冰子弹分支后添加
            else if (bullet.def.defName == "Bullet_IceWatermelon")
            {
                // 冰西瓜：0.5秒眩晕 + 3点冻伤
                stunSeconds = 0.5f;
                extraDamage = 3;
            }

            // 施加眩晕（如果有）
            if (stunSeconds > 0f)
            {
                int stunTicks = (int)(stunSeconds * 60f);
                pawn.stances.stunner.StunFor(stunTicks, source);
            }

            // 施加冻伤伤害
            if (extraDamage > 0)
            {
                DamageInfo dinfo = new DamageInfo(DamageDefOf.Frostbite, extraDamage, 0, -1, source);
                pawn.TakeDamage(dinfo);
            }

        }

        /// <summary>
        /// 烈焰子弹效果：施加烧伤Hediff + 直接点燃敌人
        /// </summary>
        private static void ApplyFireEffects(Bullet bullet, Thing victim, Thing source)
        {
            if (victim == null || !victim.Spawned) return;
            Pawn pawn = victim as Pawn;
            if (pawn == null || pawn.Dead) return;

            // ===== 1. 施加烧伤Hediff =====
            HediffDef burnDef = HediffDef.Named("Burn");
            if (burnDef != null)
            {
                Hediff burnHediff = HediffMaker.MakeHediff(burnDef, pawn);

                // 根据子弹类型设置烧伤严重度
                float severity = 0.3f;
                if (bullet.def.defName == "Bullet_FirePistol")
                    severity = 0.2f;
                else if (bullet.def.defName == "Bullet_FireRifle")
                    severity = 0.3f;
                else if (bullet.def.defName == "Bullet_FireShotgun")
                    severity = 0.4f;

                burnHediff.Severity = severity;
                pawn.health.AddHediff(burnHediff, null, null);
            }

            // ===== 2. 直接点燃敌人 =====
            float fireSize = 0.5f;
            if (bullet.def.defName == "Bullet_FirePistol")
                fireSize = 0.3f;
            else if (bullet.def.defName == "Bullet_FireRifle")
                fireSize = 0.5f;
            else if (bullet.def.defName == "Bullet_FireShotgun")
                fireSize = 0.8f;

            FireUtility.TryAttachFire(pawn, fireSize, source);
        }
    }
}