using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace TotemOfUndying
{
    public class CompProperties_Totem : CompProperties
    {
        public CompProperties_Totem()
        {
            this.compClass = typeof(CompTotem);
        }
    }

    public class CompTotem : ThingComp
    {
        private bool triggered = false;

        public bool Triggered => triggered;

        private Pawn Wearer => (parent as Apparel)?.Wearer;

        public override void CompTick()
        {
            base.CompTick();
            if (triggered) return;

            Pawn pawn = Wearer;
            if (pawn == null || pawn.Dead) return;

            if (ShouldTrigger(pawn))
            {
                TriggerEffect(pawn);
            }
        }

        private bool ShouldTrigger(Pawn pawn)
        {
            if (pawn.Dead) return false;

            // 生命百分比低于 20%
            if (pawn.health.summaryHealth.SummaryHealthPercent < 0.2f)
                return true;

            // 失血严重
            var bloodLoss = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLoss != null && bloodLoss.Severity >= 0.8f)
                return true;

            // 倒地且生命极低
            if (pawn.Downed && pawn.health.summaryHealth.SummaryHealthPercent < 0.3f)
                return true;

            return false;
        }

        public void TriggerEffect(Pawn pawn)
        {
            if (triggered) return;
            triggered = true;

            // 1. 移除所有伤害性 Hediff
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs.ToList();
            foreach (var hediff in hediffs)
            {
                if (hediff is Hediff_Injury || hediff.def == HediffDefOf.BloodLoss)
                {
                    pawn.health.RemoveHediff(hediff);
                }
                else if (hediff.def.defName == "Pain")  // 兼容不同版本的 Pain 定义
                {
                    pawn.health.RemoveHediff(hediff);
                }
            }

            // 2. 额外确保清除失血
            var bloodLoss = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
            if (bloodLoss != null) pawn.health.RemoveHediff(bloodLoss);

            // 3. 唤醒倒地的小人
            if (pawn.Downed)
            {
                pawn.jobs?.StopAll();
                // 使用默认的 Wait 工作
                pawn.jobs?.StartJob(new Job(JobDefOf.Wait), JobCondition.InterruptForced);
            }

            // 4. 特效
            FleckMaker.ThrowSmoke(pawn.DrawPos, pawn.Map, 1.5f);
            FleckMaker.ThrowMicroSparks(pawn.DrawPos, pawn.Map);

            // 5. 销毁图腾
            parent.Destroy(DestroyMode.Vanish);

            // 6. 消息
            Messages.Message($"{pawn.LabelShort} 被不死图腾救活了！", pawn, MessageTypeDefOf.PositiveEvent);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref triggered, "triggered", false);
        }
    }
}