using GameFramework.Generic;

namespace GameFramework.ObjectPoolService
{
    public class LifeTimeData : Data
    {
        public float lifeTime;
        
        public override void Clear()
        {
            lifeTime = 0.0f;
        }
    }
}