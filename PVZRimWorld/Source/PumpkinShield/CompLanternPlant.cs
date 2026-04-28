using RimWorld;
using Verse;
using System.Collections.Generic;

namespace PumpkinShield
{
    public class CompProperties_LanternPlant : CompProperties
    {
        public float radius = 4f;

        public CompProperties_LanternPlant()
        {
            this.compClass = typeof(CompLanternPlant);
        }
    }

    /// <summary>
    /// 灯笼草组件：每 250 ticks 检测一次殖民者距离，在范围内时添加记忆。
    /// 记忆使用 Thought_Memory，自带持续时间（1小时），离开后自动消失。
    /// </summary>
    public class CompLanternPlant : ThingComp
    {
        public CompProperties_LanternPlant Props => (CompProperties_LanternPlant)props;

        // 用于记录上次检测时在范围内的殖民者，便于移除记忆（可选，但为了精准，我们保留）
        private HashSet<Pawn> lastInRange = new HashSet<Pawn>();

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (parent == null || !parent.Spawned)
            {
                // 植物不存在时，清除所有相关记忆（可选，但记忆本身会超时消失）
                RemoveAllMemories();
                return;
            }

            List<Pawn> colonists = parent.Map.mapPawns.FreeColonistsSpawned;
            HashSet<Pawn> nowInRange = new HashSet<Pawn>();

            // 检测当前范围内的殖民者，添加/刷新记忆
            foreach (Pawn pawn in colonists)
            {
                if (pawn.Position.DistanceTo(parent.Position) <= Props.radius)
                {
                    nowInRange.Add(pawn);
                    AddMemory(pawn);
                }
            }

            // 对于上次在范围内但这次不在的殖民者，我们不需要主动移除记忆，
            // 因为记忆会在一小时后自动消失。但为了立即清除（如果希望离开立即消失），可以主动移除。
            // 根据您的要求，离开后记忆应持续一小时，所以这里不需要主动移除。
            // 但如果离开后想要立即消失，可以取消下面注释。
            /*
            foreach (Pawn pawn in lastInRange)
            {
                if (!nowInRange.Contains(pawn))
                {
                    RemoveMemory(pawn);
                }
            }
            */

            lastInRange = nowInRange;
        }

        private void AddMemory(Pawn pawn)
        {
            ThoughtDef def = DefDatabase<ThoughtDef>.GetNamed("LanternPlantNearby");
            if (def != null && pawn.needs?.mood?.thoughts != null)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(def);
                // TryGainMemory 会自动处理 stackLimit 和持续时间刷新
            }
        }

        private void RemoveMemory(Pawn pawn)
        {
            ThoughtDef def = DefDatabase<ThoughtDef>.GetNamed("LanternPlantNearby");
            if (def != null && pawn.needs?.mood?.thoughts != null)
            {
                pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(def);
            }
        }

        private void RemoveAllMemories()
        {
            foreach (Pawn pawn in lastInRange)
            {
                RemoveMemory(pawn);
            }
            lastInRange.Clear();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            RemoveAllMemories();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref lastInRange, "lastInRange", LookMode.Reference);
            if (lastInRange == null)
                lastInRange = new HashSet<Pawn>();
        }
    }
}