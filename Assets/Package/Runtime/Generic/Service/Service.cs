using UnityEngine;

namespace GameFramework.Generic
{
    public abstract class Service : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (!Global.AddService(this))
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            Global.RemoveService(this);
        }
    }
}