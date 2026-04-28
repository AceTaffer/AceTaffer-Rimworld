using RimWorld;
using Verse;

namespace PumpkinShield
{
    // ========== 自定义毁灭菇植物类 ==========
    /// <summary>
    /// 毁灭菇的自定义植物类，用于标记是否因收获而销毁。
    /// 必须重写 PlantCollected 方法以捕获收获事件。
    /// </summary>
    public class Plant_DoomShroom : Plant
    {
        // 标记是否是因为收获而销毁（true = 被殖民者收获）
        private bool wasHarvested = false;

        /// <summary>
        /// 当植物被殖民者收获时调用（RimWorld 1.4+ 版本签名）。
        /// </summary>
        /// <param name="by">执行收获的殖民者</param>
        /// <param name="mode">收获模式（通常为 Harvest）</param>
        public override void PlantCollected(Pawn by, PlantDestructionMode mode)
        {
            wasHarvested = true;      // 设置收获标记
            base.PlantCollected(by, mode); // 调用基类方法，完成默认收获逻辑
        }

        /// <summary>
        /// 公开只读属性，供组件检查是否因收获而销毁。
        /// </summary>
        public bool WasHarvested => wasHarvested;

        // 注意：wasHarvested 不需要保存到存档，因为植物销毁后就不再需要此标记
    }

    // ========== 爆炸组件属性类 ==========
    /// <summary>
    /// 定义死亡爆炸组件的可配置属性。
    /// </summary>
    public class CompProperties_ExplodeOnDeath : CompProperties
    {
        /// <summary>爆炸半径（格）</summary>
        public float explosionRadius = 5.9f;

        /// <summary>爆炸伤害值</summary>
        public int explosionDamage = 100;

        /// <summary>爆炸伤害类型（如 DamageDefOf.Bomb）。默认设为 null，在 ResolveReferences 中赋值以避免 DefOf 未初始化问题。</summary>
        public DamageDef explosionDamageDef;

        /// <summary>触发爆炸所需的最小生长进度（0.0~1.0）</summary>
        public float minGrowthToExplode = 1.0f;

        public CompProperties_ExplodeOnDeath()
        {
            this.compClass = typeof(CompExplodeOnDeath);
        }

        /// <summary>
        /// 在所有 Def 加载完成后解析引用。在此处设置默认伤害类型，确保此时 DamageDefOf 已初始化。
        /// </summary>
        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (explosionDamageDef == null)
                explosionDamageDef = DamageDefOf.Bomb; // 安全使用已初始化的 DefOf
        }
    }

    // ========== 爆炸组件 ==========
    /// <summary>
    /// 当植物死亡（被摧毁）时引发爆炸。收获时不会爆炸。
    /// 支持多种自定义爆炸植物（毁灭菇、火爆辣椒、寒冰菇）。
    /// </summary>
    public class CompExplodeOnDeath : ThingComp
    {
        /// <summary>快捷访问属性，获取强类型的属性实例</summary>
        public CompProperties_ExplodeOnDeath Props => (CompProperties_ExplodeOnDeath)props;

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            // 获取地图（优先使用当前地图，否则用先前地图）
            Map map = parent.MapHeld ?? previousMap;
            if (map == null) return;

            // 获取植物组件（如果不是植物，则直接返回）
            Plant plant = parent as Plant;
            if (plant == null) return;

            // 只有生长进度达到阈值时才考虑爆炸
            if (plant.Growth >= Props.minGrowthToExplode)
            {
                // 🔥 关键判断：检查是否因收获而销毁（通过自定义植物类的 WasHarvested 属性）
                bool harvested = false;

                // 毁灭菇（定义在本文件内）
                if (parent is Plant_DoomShroom doomShroom)
                    harvested = doomShroom.WasHarvested;
                // 火爆辣椒（定义在 Plant_HotPepper.cs 中）
                else if (parent is Plant_HotPepper hotPepper)
                    harvested = hotPepper.WasHarvested;
                // 寒冰菇（定义在 CompFreezeExplosion.cs 中）
                else if (parent is Plant_IceShroom iceShroom)
                    harvested = iceShroom.WasHarvested;
                // 如有更多爆炸植物，可继续添加 else if 分支

                if (harvested)
                    return; // 收获时不触发爆炸

                // 执行爆炸
                GenExplosion.DoExplosion(
                    parent.PositionHeld,      // 爆炸中心
                    map,                       // 地图
                    Props.explosionRadius,     // 半径
                    Props.explosionDamageDef,  // 伤害类型（已通过 ResolveReferences 确保非空）
                    null,                       // 引发者（null表示无）
                    Props.explosionDamage,      // 伤害值
                    -1f                         // 护甲穿透（-1表示使用伤害类型的默认值）
                );
            }
        }
    }
}