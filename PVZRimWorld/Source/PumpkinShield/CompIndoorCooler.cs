using RimWorld;
using Verse;

namespace PumpkinShield
{
    // ========== 室内降温组件属性 ==========
    public class CompProperties_IndoorCooler : CompProperties
    {
        /// <summary>制冷功率（热量/秒，负值表示制冷）</summary>
        public float coolingPower = -1000f; // 足够强的冷量，使室温快速降至-20℃

        public CompProperties_IndoorCooler()
        {
            this.compClass = typeof(CompIndoorCooler);
        }
    }

    // ========== 室内降温组件 ==========
    /// <summary>
    /// 当植物处于室内（有屋顶）时，持续向所在单元格推送冷气，使室温降至-20℃左右。
    /// 使用 CompTickRare 降低性能开销。
    /// </summary>
    public class CompIndoorCooler : ThingComp
    {
        public CompProperties_IndoorCooler Props => (CompProperties_IndoorCooler)props;

        public override void CompTickRare()
        {
            base.CompTickRare();

            // 确保植物已生成且未销毁
            if (parent == null || !parent.Spawned) return;

            Map map = parent.Map;
            if (map == null) return;

            // 检查是否在室内（有屋顶覆盖）
            if (parent.Position.Roofed(map))
            {
                // 推送冷气，每250tick一次（CompTickRare间隔），乘以250换算为每秒
                GenTemperature.PushHeat(parent.Position, map, Props.coolingPower * 250f);
            }
        }
    }
}