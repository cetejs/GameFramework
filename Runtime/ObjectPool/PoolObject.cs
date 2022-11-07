using System.Collections;
using GameFramework.Generic;
using UnityEngine;

namespace GameFramework.ObjectPoolService
{
    [DisallowMultipleComponent]
    public abstract class PoolObject : MonoBehaviour
    {
        [SerializeField] [ReadOnly]
        private ObjectPool pool;
        [SerializeField] [ReadOnly]
        private bool isReleased;
        [SerializeField]
        private float lifeTime;
        private Data data;
        private float defaultLifeTime;
        private float lifeSeconds;
        private WaitForSeconds waitForLifeSeconds;

        public bool IsReleased
        {
            get { return isReleased; }
        }

        public T GetData<T>() where T : Data
        {
            return data as T;
        }

        public void Init(ObjectPool pool)
        {
            this.pool = pool;
            OnInit();
        }

        public void WakeUp(Data data)
        {
            this.data = data;
            isReleased = false;
            OnWakeUp();

            if (data is LifeTimeData lifeData)
            {
                defaultLifeTime = lifeTime;
                lifeTime = lifeData.lifeTime;
            }

            if (lifeTime > 0)
            {
                if (waitForLifeSeconds == null || !Mathf.Approximately(lifeSeconds, lifeTime))
                {
                    lifeSeconds = lifeTime;
                    waitForLifeSeconds = new WaitForSeconds(lifeSeconds);
                }

                StartCoroutine(AutoRelease());
            }
        }

        public void Sleep()
        {
            StopAllCoroutines();
            OnSleep();
            isReleased = true;
            if (data != null)
            {
                if (data is LifeTimeData)
                {
                    lifeTime = defaultLifeTime;
                }

                ReferencePool.Release(data);
                data = null;
            }
        }

        public void Release()
        {
            if (isReleased)
            {
                return;
            }

            pool.Release(this);
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnWakeUp()
        {
        }

        protected virtual void OnSleep()
        {
        }

        private IEnumerator AutoRelease()
        {
            yield return waitForLifeSeconds;
            Release();
        }
    }
}