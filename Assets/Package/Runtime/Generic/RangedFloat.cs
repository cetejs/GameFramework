using System;

namespace GameFramework.Generic
{
    [Serializable]
    public class RangedFloat
    {
        public float minValue;
        public float maxValue;

        public RangedFloat(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}