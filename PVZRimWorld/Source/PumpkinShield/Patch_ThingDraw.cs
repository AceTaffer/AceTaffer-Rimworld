using HarmonyLib;
using Verse;

namespace PumpkinShield
{
    // 直接指定要修补的方法：Thing.Draw
    [HarmonyPatch(typeof(Thing), "Draw")]
    public static class Patch_ThingDraw
    {
        /// <summary>
        /// 在应用补丁前检查目标方法是否存在，不存在则跳过（防止模组加载失败）
        /// </summary>
        public static bool Prepare()
        {
            var method = AccessTools.Method(typeof(Thing), "Draw");
            if (method == null)
            {
                Log.Warning("[PVZRimWorld] Thing.Draw method not found. NutBowl rotation patch will be skipped.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Prefix 补丁：拦截绘制，应用旋转角度
        /// </summary>
        public static bool Prefix(Thing __instance)
        {
            // 仅处理我们的坚果保龄球投射物
            if (__instance.def.defName != "Projectile_NutBowl")
                return true;

            // 获取旋转组件
            var rotComp = __instance.TryGetComp<CompNutBowlRotator>();
            if (rotComp == null)
                return true;

            // 手动绘制，应用旋转角度
            if (__instance.Graphic != null)
            {
                __instance.Graphic.Draw(
                    __instance.DrawPos,
                    Rot4.North,
                    __instance,
                    rotComp.RotationAngle
                );
            }

            // 返回 false 跳过原绘制，避免双重绘制
            return false;
        }
    }
}