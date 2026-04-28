using Verse;

namespace PumpkinShield
{
    public class CompProperties_NutBowlRotator : CompProperties
    {
        public CompProperties_NutBowlRotator()
        {
            this.compClass = typeof(CompNutBowlRotator);
        }
    }

    public class CompNutBowlRotator : ThingComp
    {
        private float rotationAngle;

        public float RotationAngle => rotationAngle;

        public void UpdateRotation()
        {
            rotationAngle += 10f; // 每帧 10 度 → 600 度/秒（1.67 圈/秒），可调整
            if (rotationAngle > 360f)
                rotationAngle -= 360f;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref rotationAngle, "rotationAngle", 0f);
        }
    }
}