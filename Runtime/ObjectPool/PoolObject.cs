using UnityEngine;

namespace GameFramework
{
    [DisallowMultipleComponent]
    public abstract class PoolObject : MonoBehaviour
    {
        [SerializeField] [ReadOnly]
        private ObjectPool pool;
        [SerializeField]
        private float lifeTime = -1f;
        private float defaultLifeTime;
        private float lifeTimer;

        public float LifeTime
        {
            get { return lifeTime; }
            set
            {
                lifeTimer = value;
                lifeTime = value;
            }
        }

        public bool IsReleased { get; private set; }

        protected virtual void Update()
        {
            if (lifeTime <= -1)
            {
                return;
            }

            if (lifeTimer > 0)
            {
                lifeTimer -= Time.deltaTime;
            }
            else
            {
                Release();
            }
        }

        public void Release()
        {
            if (IsReleased)
            {
                return;
            }

            pool.Release(this);
        }

        public virtual void OnInit(ObjectPool pool)
        {
            this.pool = pool;
        }

        public virtual void OnWakeUp()
        {
            IsReleased = false;
        }

        public virtual void OnSleep()
        {
            IsReleased = true;
        }
    }
}