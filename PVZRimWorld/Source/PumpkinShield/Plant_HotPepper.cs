using System;
using RimWorld;
using Verse;

namespace PumpkinShield
{
    /// <summary>
    /// 火爆辣椒的自定义植物类，用于标记是否因收获而销毁。
    /// 当殖民者收获时，设置 wasHarvested = true，爆炸组件据此跳过爆炸。
    /// </summary>
    public class Plant_HotPepper : Plant
    {
        private bool wasHarvested = false;

        public override void PlantCollected(Pawn by, PlantDestructionMode mode)
        {
            wasHarvested = true;          // 标记为收获
            base.PlantCollected(by, mode); // 调用基类方法，完成默认收获逻辑
        }

        public bool WasHarvested => wasHarvested;
    }
}